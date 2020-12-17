using Google.Apis.Util;
using Hasebni.Base;
using Hasebni.Main.Dto.Member;
using Hasebni.Main.Dto.Notification;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.Model.Main;
using Hasebni.SqlServer.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace Hasebni.Main.Data.Repository
{
    public class MemberRepository:HasebniRepository , IMemberRepository
    {
        //private readonly IGroupRepository groupRepository;

        public MemberRepository(//IGroupRepository groupRepository,
            HasebniDbContext context):base(context)
        {
            //this.groupRepository = groupRepository;
        }


        public async Task<OperationResult<MemberInfoDto>> GetAllMemberInGroup(int id)
        {
            OperationResult<MemberInfoDto> operation = new OperationResult<MemberInfoDto>();
            try
            {
                var members = await Context.Members
                    .Include(m=>m.Profile).ThenInclude(m=>m.HUser)
                    .Where(m => (!m.DateDeleted.HasValue) && (m.GroupFk == id))
                    .Select(m=> new MemberInfoDto
                    {
                        Id = m.Id,
                        FirstName = m.Profile.FirstName,
                        LastName = m.Profile.LastName,
                        UserName = m.Profile.HUser.UserName,
                        Balance = m.Balance,
                        DateAdded = m.DateAdded,
                        IsAdmin = m.IsAdmin,
                    }).ToListAsync();
               
               operation.OperationResultType = OperationResultTypes.Success;
               operation.IEnumerableResult = members;
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<long>> GetBalancesForUsersInGroup(int groupId)
        {
            OperationResult<long> operation = new OperationResult<long>();
            try
            {
                var members = await Context.Members
                    .Where(m => (!m.DateDeleted.HasValue) && (m.GroupFk == groupId))
                    .ToListAsync();
                if(members != null)
                {
                    List<long> data = members.Select(m => m.Balance).ToList();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.IEnumerableResult = data;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.IEnumerableResult = null;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<double>> GetBalanceUser(int memberId)
        {
            OperationResult<double> operation = new OperationResult<double>();
            try
            {
                var member = await Context.Members
                       .Where(m => (!m.DateDeleted.HasValue) && (m.Id == memberId))
                       .SingleOrDefaultAsync();
                if (member != null)
                {
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = member.Balance;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = 0;
                }

            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
            
        }

        public async Task<OperationResult<bool>> LeaveGroup(int memberId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var member = await Context.Members
                    .Where(m => (!m.DateDeleted.HasValue) && (m.Id == memberId))
                    .SingleOrDefaultAsync();
                if(member == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
                else
                {
                    if (member.IsAdmin)
                    {
                        var newAdmin = Context.Members
                            .Where(m => (!m.DateDeleted.HasValue) && (m.GroupFk == member.GroupFk))
                            .OrderByDescending(m => m.DateAdded)
                            .Take(1).SingleOrDefault();
                        newAdmin.IsAdmin = true;
                        Context.Members.Update(newAdmin);
                    }
                    member.DateDeleted = DateTime.Now;
                    Context.Members.Update(member);
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = true;
                }
            }

            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        #region Admin

        public async Task<OperationResult<MemberInfoDto>> AddMembers(int groupId, IEnumerable<string> users)
        {
            OperationResult<MemberInfoDto> operation = new OperationResult<MemberInfoDto>();
            try
            {
                Profile profile;
                List<int> Ids = new List<int>();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        profile = await Context.Profiles.
                            Include(p => p.HUser)
                            .Where(p => p.HUser.UserName == user)
                            .SingleOrDefaultAsync();
                        Ids.Add(profile.Id);
                    }

                    List<Member> members = new List<Member>();
                    foreach (var Id in Ids)
                    {
                        members.Add(new Member
                        {
                            IsAdmin = false,
                            Balance = 0,
                            DateAdded = DateTimeOffset.UtcNow,
                            GroupFk = groupId,
                            ProfileFk = Id
                        });
                    }
                    Context.Members.AddRange(members);
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.IEnumerableResult = members.Select(m => new MemberInfoDto
                    {
                        FirstName = m.Profile.FirstName,
                        LastName = m.Profile.LastName,
                        Balance = 0,
                        DateAdded = m.DateAdded,
                        IsAdmin = false,
                        UserName = m.Profile.HUser.UserName
                    });
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.IEnumerableResult = null;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> ChangeAdmin(ChangeAdminDto changeAdminDto)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var group = await Context.Groups.
                    Where(g => (!g.DateDeleted.HasValue) && (g.Id == changeAdminDto.GroupId))
                    .SingleOrDefaultAsync();
                var oAdmin = await Context.Members.
                    Where(m => (!m.DateDeleted.HasValue) && (m.Id == changeAdminDto.AdminId))
                    .SingleOrDefaultAsync();
                var nAdmin = Context.Members
                    .Where(m => m.Id == Context.Profiles
                                            .Where(p => p.HUserFk == (Context.Users
                                                                        .Where(u => u.UserName == changeAdminDto.NewAdmin)
                                                                        .SingleOrDefault()).Id)
                                            .SingleOrDefault().Id)
                    .SingleOrDefault();


                if (group == null || oAdmin == null || nAdmin == null)
                {
                    operation.OperationResultType = OperationResultTypes.Failed;
                    operation.Result = false;
                }
                else
                {
                    oAdmin.IsAdmin = false;
                    nAdmin.IsAdmin = true;
                    Context.Members.Update(oAdmin);
                    Context.Members.Update(nAdmin);
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = true;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
                operation.Result = false;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> DeleteMember(int memberId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var groupId = Context.Groups
                    .Where(g => (g.Members.Any(m => m.Id == memberId)) && (!g.DateDeleted.HasValue))
                    .SingleOrDefault().Id;

                var result = await IsAllBalancesZero(groupId);
                if (result.Result)
                {
                    var member = await Context.Members
                        .Where(m => (!m.IsDeleted) && (m.Id == memberId))
                        .SingleOrDefaultAsync();
                    if (member != null)
                    {
                        member.DateDeleted = DateTime.Now;
                        await Context.SaveChangesAsync();
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = true;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.NotExist;
                        operation.Result = false;
                    }
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.Forbidden;
                    operation.Result = false;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> IsAllBalancesZero(int groupId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                bool state = false;
                var result = await GetBalancesForUsersInGroup(groupId);
                if (result.IEnumerableResult != null)
                {
                    foreach (var balance in result.IEnumerableResult)
                    {
                        if (balance != 0)
                        {
                            state = true;
                            break;
                        }
                    }
                    if (state)
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = false;
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        operation.Result = true;
                    }
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.Failed;
                    operation.Result = false;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<NotificationInfoDto>> GetIgnoredNotifications(int groupId)
        {
            OperationResult<NotificationInfoDto> operation = new OperationResult<NotificationInfoDto>();
            try
            {
                //get needed group 
                var group = await Context.Groups
                            .Where(g => (!g.DateDeleted.HasValue) && (g.Id == groupId))
                            .SingleOrDefaultAsync();
                //get Ignored notifications for this group
                var notifications = await Context.Notifications
                    .Where(n => (!n.DateDeleted.HasValue) && (n.State == 0) && 
                        (
                            group.Members.Select(m => m.Id)
                            .Contains(n.FromMemberId)))
                    .ToListAsync();

                if(notifications != null)
                {
                    //notification :  FromMemberId , ToMemberId , purchaseFK ,  

                    //var x = notifications.Select(n => new int{} (n.FromMemberId));
                    
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.IEnumerableResult = notifications.Select(n => new NotificationInfoDto
                    {
                        GroupName = group.Name,
                        AmountAdded = n.AmountAdded,
                        NotificationId = n.Id,
                        ItemName = Context.Items
                        .Where(i => (i.Id == 
                                        (Context.Purchases
                                        .Where(p=>p.Id==n.PurchaseFK)
                                        .SingleOrDefault().ItemFk)))
                        .SingleOrDefault().Name,

                        BuyerName = Context.Profiles
                        .Where(p => (!p.DateDeleted.HasValue) && (p.Id ==
                                                    (Context.Members
                                                    .Where(m => (m.Id == n.FromMemberId) && (!m.DateDeleted.HasValue))
                                                    .SingleOrDefault().ProfileFk)))
                            .SingleOrDefault().FirstName
                    });;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                }

            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<NotificationInfoDto>> GetRefusedNotifications(int groupId)
        {
            OperationResult<NotificationInfoDto> operation = new OperationResult<NotificationInfoDto>();
            try
            {
                //get needed group 
                var group = await Context.Groups
                            .Where(g => (!g.DateDeleted.HasValue) && (g.Id == groupId))
                            .SingleOrDefaultAsync();
                //get Ignored notifications for this group
                var notifications = await Context.Notifications
                    .Where(n => (!n.DateDeleted.HasValue) && (n.State == 2) &&
                        (
                            group.Members.Select(m => m.Id)
                            .Contains(n.FromMemberId)))
                    .ToListAsync();

                if (notifications != null)
                {
                    //notification :  FromMemberId , ToMemberId , purchaseFK ,  

                    //var x = notifications.Select(n => new int{} (n.FromMemberId));

                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.IEnumerableResult = notifications.Select(n => new NotificationInfoDto
                    {
                        GroupName = group.Name,
                        AmountAdded = n.AmountAdded,
                        NotificationId = n.Id,
                        ItemName = Context.Items
                        .Where(i => (i.Id ==
                                        (Context.Purchases
                                        .Where(p => p.Id == n.PurchaseFK)
                                        .SingleOrDefault().ItemFk)))
                        .SingleOrDefault().Name,

                        BuyerName = Context.Profiles
                        .Where(p => (!p.DateDeleted.HasValue) && (p.Id ==
                                                    (Context.Members
                                                    .Where(m => (m.Id == n.FromMemberId) && (!m.DateDeleted.HasValue))
                                                    .SingleOrDefault().ProfileFk)))
                            .SingleOrDefault().FirstName
                    }); ;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                }

            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> IsAdmin(int memberId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var member = await Context.Members
                    .Where(m => (m.Id == memberId) && (!m.DateDeleted.HasValue))
                    .SingleOrDefaultAsync();
                if (member == null) 
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;

                }
                else
                {
                    operation.Result = member.IsAdmin;
                    operation.OperationResultType = OperationResultTypes.Success;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<bool> SetAllBalancesZero(int groupId)
        {
            var members = Context.Members.Where(g => (!g.DateDeleted.HasValue) && (g.Id == groupId));
            try
            {
                if (members.Count() != 0)
                {
                    foreach (var item in members)
                    {
                        item.Balance = 0;
                    }
                    Context.UpdateRange(members);
                    await Context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<OperationResult<bool>> IsAlwaysAccepted(int memberId, bool state)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var member = await Context.Members
                .Where(m => (!m.DateDeleted.HasValue) && (m.Id == memberId))
                .SingleOrDefaultAsync();
                if (member != null)
                {
                    member.IsAlwaysAccepted = state;
                    Context.Members.Update(member);
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = true;
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                }
            }
            catch (Exception)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }
        #endregion
    }
}
