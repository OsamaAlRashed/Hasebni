using Hasebni.Base;
using Hasebni.Main.Dto;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.Model.Main;
using Hasebni.SqlServer.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hasebni.Main.Data.Repository
{
    public class GroupRepository:HasebniRepository , IGroupRepository
    {
        private readonly IMemberRepository memberRepository;

        public GroupRepository(IMemberRepository memberRepository,
            HasebniDbContext context) :base(context)
        {
            this.memberRepository = memberRepository;
        }

        public async Task<OperationResult<int>> CreateGroup(CreateGroupDto createGroupDto , string myurl)
        {
            OperationResult<int> operation = new OperationResult<int>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    #region SavePhoto
                    string PathDB = String.Empty;
                    if (createGroupDto.GroupPhoto != null)
                    {
                        try
                        {
                            PathDB = Guid.NewGuid().ToString() + Path.GetExtension(createGroupDto.GroupPhoto.FileName);
                            var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Images/Group", PathDB);

                            PathDB = $"{myurl}/Images/Group/{PathDB}";
                            using (var bits = new FileStream(path, FileMode.Create))
                            {
                                // await bits.WriteAsync(registerUserDto.UserPhoto, 0, registerUserDto.UserPhoto.Length);
                                await createGroupDto.GroupPhoto.CopyToAsync(bits);
                            }
                        }
                        catch (Exception)
                        {
                            operation.OperationResultType = OperationResultTypes.Exception;
                        }
                    }
                    #endregion

                    //CreateGroup
                    var group = new Group
                    {
                        Name = createGroupDto.Name,
                        Description = createGroupDto.Description,
                        ImagePath = PathDB,
                    };
                    Context.Groups.Add(group);
                    await Context.SaveChangesAsync();

                    var intilizeAdmin = await IntilizeAdmin(group.Id, createGroupDto.CreaterId);
                    var addMembers = await memberRepository.AddMembers(group.Id, createGroupDto.Members);

                    if (intilizeAdmin && (addMembers.OperationResultType == OperationResultTypes.Success
                        || addMembers.OperationResultType == OperationResultTypes.NotExist))
                    {
                        operation.OperationResultType = OperationResultTypes.Success;
                        //operation.Result = new CreateGroupDto {
                        //    Members = addMembers.IEnumerableResult.Select(m=> new string(m.FirstName + " " + m.LastName)),
                        //    CreaterId = createGroupDto.CreaterId,
                        //    Description = group.Description,
                        //    Name = group.Name,
                        //    Id = group.Id
                        //};
                        operation.Result = group.Id;
                        transaction.Commit();
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Failed;
                        operation.Result = 0;
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    operation.OperationResultType = OperationResultTypes.Exception;
                    operation.Exception = ex;
                    transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<bool>> DeleteGroup(int id)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var group = await Context.Groups
                    .Where(g => (!g.DateDeleted.HasValue) && (g.Id == id))
                    .SingleOrDefaultAsync();
                if(group == null)
                {
                    operation.OperationResultType = OperationResultTypes.Failed;
                    operation.Result = false;
                }
                else
                {
                    group.DateDeleted = DateTimeOffset.UtcNow;
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

        public async Task<OperationResult<GroupInfoDtoResponse>> SearchGroups(string text)
        {
            OperationResult<GroupInfoDtoResponse> operation = new OperationResult<GroupInfoDtoResponse>();
            try
            {
                var data = await Context.Groups
                    .Where(g => (!g.DateDeleted.HasValue) && (g.Name.Contains(text)))
                    .Select(g => new GroupInfoDtoResponse
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        ImagePath = g.ImagePath
                    }).ToListAsync();
                operation.OperationResultType = OperationResultTypes.Success;
                operation.IEnumerableResult = data;
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<GroupInfoDtoResponse>> UpdateGroup(GroupInfoDto groupInfoDto , string myurl)
        {
            OperationResult<GroupInfoDtoResponse> operation = new OperationResult<GroupInfoDtoResponse>();
            try
            {
                var group = await Context.Groups
                    .Where(g => (!g.DateDeleted.HasValue) && (g.Id == groupInfoDto.Id))
                    .SingleOrDefaultAsync();
                if (group != null)
                {
                    if (groupInfoDto.GroupPhoto != null)
                    {
                        //delete old image
                        string PathDB = String.Empty;
                        string oldPath = group.ImagePath;
                        string getGuid = oldPath.Substring(oldPath.LastIndexOf('/') + 1);
                        string deletePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Images/Group", getGuid);
                        try
                        {
                            File.Delete(deletePath);
                        }
                        catch { }

                        //add new image and update
                        try
                        {
                            PathDB = Guid.NewGuid().ToString() + Path.GetExtension(groupInfoDto.GroupPhoto.FileName);
                            var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Images/Group", PathDB);

                            PathDB = $"{myurl}/Images/Group/{PathDB}";
                            using (var bits = new FileStream(path, FileMode.Create))
                            {
                                await groupInfoDto.GroupPhoto.CopyToAsync(bits);
                            }
                            group.Name = groupInfoDto.Name;
                            group.Description = groupInfoDto.Description;
                            group.ImagePath = PathDB;
                            Context.Groups.Update(group);
                            await Context.SaveChangesAsync();
                        }

                        catch (Exception)
                        {
                            operation.OperationResultType = OperationResultTypes.Exception;
                        }
                    }
                    else
                    {
                        group.Name = groupInfoDto.Name;
                        group.Description = groupInfoDto.Description;
                        Context.Groups.Update(group);
                        await Context.SaveChangesAsync();
                    }
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = new GroupInfoDtoResponse
                    {
                        Id = group.Id,
                        Name = group.Name,
                        Description = group.Description,
                        ImagePath = group.ImagePath
                    };
                }
                else
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = null;
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<GroupInfoDtoResponse>> GetAllGroups()
        {
            OperationResult<GroupInfoDtoResponse> operation = new OperationResult<GroupInfoDtoResponse>();
            try
            {
                var data = await Context.Groups
                    .Where(g => (!g.DateDeleted.HasValue))
                    .Select(g => new GroupInfoDtoResponse
                    {
                        Id = g.Id,
                        Description = g.Description,
                        ImagePath = g.ImagePath,
                        Name = g.Name
                    }).ToListAsync();
                operation.OperationResultType = OperationResultTypes.Success;
                operation.IEnumerableResult = data;
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<GroupInfoDtoResponse>> GetAllGroupsForUser(int userId)
        {
            OperationResult<GroupInfoDtoResponse> operation = new OperationResult<GroupInfoDtoResponse>();
            try
            {
                var members = await Context.Members
                    .Where(m => (m.ProfileFk == userId) && (!m.DateDeleted.HasValue))
                    .ToListAsync();
                List<int> ids = new List<int>();
                foreach (var item in members)
                {
                    ids.Add(item.Id);
                }
                var data = await Context.Groups.Include(m => m.Members)
                    .Where(g => (!g.DateDeleted.HasValue) && (ids.Contains(g.Id)))
                    .Select(g => new GroupInfoDtoResponse
                    {
                        Id = g.Id,
                        Description = g.Description,
                        ImagePath = g.ImagePath,
                        Name = g.Name
                    }).ToListAsync();
                operation.OperationResultType = OperationResultTypes.Success;
                operation.IEnumerableResult = data;
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        private async Task<bool> IntilizeAdmin(int groupId , int userId)
        {
            try
            {
                //ids valid
                var user = await Context.Profiles
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();
                var group = await Context.Groups
               .Where(g => g.Id == groupId)
               .SingleOrDefaultAsync();
                if (user == null || group == null)
                {
                    return false;
                }
                //add admin
                Context.Members.Add(new Member
                {
                    Balance = 0,
                    IsAdmin = true,
                    DateAdded = DateTimeOffset.UtcNow,
                    GroupFk = groupId,
                    ProfileFk = user.Id,
                });
                await Context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
