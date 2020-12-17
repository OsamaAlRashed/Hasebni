using Hasebni.Base;
using Hasebni.Main.Dto.Notification;
using Hasebni.Main.Dto.Purchase;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.Model.Main;
using Hasebni.Model.Setting;
using Hasebni.SqlServer.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Hasebni.Main.Data.Repository
{
    public class PurchaseRepository:HasebniRepository , IPurchaseRepository
    {
        private readonly INotificationRepository notificationRepository;
        private readonly IMemberRepository memberRepository;

        public PurchaseRepository(HasebniDbContext context,
            INotificationRepository notificationRepository,
            IMemberRepository memberRepository) :base(context)
        {
            this.notificationRepository = notificationRepository;
            this.memberRepository = memberRepository;
        }


        public async Task<OperationResult<PurchaseInfoDto>> AddPurchase(PurchaseInfoDto purchaseInfoDto)
        {
            
            OperationResult<PurchaseInfoDto> operation = new OperationResult<PurchaseInfoDto>();
            using (var Transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Purchase purchase;
                    //new item
                    if (purchaseInfoDto.ItemId == 0)
                    {
                        var newItem = new Item
                        {
                            GroupFk = purchaseInfoDto.GroupId,
                            Name = purchaseInfoDto.NewItemName,
                        };
                        Context.Items.Add(newItem);
                        await Context.SaveChangesAsync();
                        purchase = new Purchase
                        {
                            ItemFk = newItem.Id,
                            Price = purchaseInfoDto.Price,
                            DatePurchase = DateTimeOffset.UtcNow,
                        };
                        Context.Purchases.Add(purchase);
                        await Context.SaveChangesAsync();
                    }

                    //item from select
                    else
                    {
                        purchase = new Purchase
                        {
                            ItemFk = purchaseInfoDto.ItemId,
                            Price = purchaseInfoDto.Price,
                            DatePurchase = DateTimeOffset.UtcNow,
                        };
                        Context.Purchases.Add(purchase);
                        await Context.SaveChangesAsync();
                    }

                    //add subscribers and notifcations
                    List<Subscriber> subscribers = new List<Subscriber>();
                    List<Notification> notifications = new List<Notification>();
                    int i = 0;
                    int currentValue = purchaseInfoDto.Price % (purchaseInfoDto.MembersIds.Count() + 1);
                    int set = purchaseInfoDto.Price / (purchaseInfoDto.MembersIds.Count() + 1);
                    foreach (var id in purchaseInfoDto.MembersIds)
                    {
                        ++i;
                        subscribers.Add(new Subscriber
                        {
                            IsBuyer = false,
                            IsSubscriber = true,
                            MemberFk = id,
                            PurchaseFk = purchase.Id,
                        });
                        if (i <= currentValue)
                        {
                            set = currentValue + 1;
                        }
                        else
                        {
                            set = currentValue;
                        }
                        notifications.Add(new Notification
                        {
                            FromMemberId = purchaseInfoDto.ByerId,
                            ToMemberId = id,
                            AmountAdded = set,
                            PurchaseFK = purchase.Id,
                        });
                    }
                    Context.Subscribers.AddRange(subscribers);
                    await Context.SaveChangesAsync();
                    Context.Notifications.AddRange(notifications);
                    await Context.SaveChangesAsync();
                    //add buyer
                    Context.Subscribers.Add(new Subscriber
                    {
                        IsBuyer = true,
                        IsSubscriber = true,
                        MemberFk = purchaseInfoDto.ByerId,
                        PurchaseFk = purchase.Id
                    });
                    await Context.SaveChangesAsync();

                    var autoAC = await Context.Members
                        .Where(m => (purchaseInfoDto.MembersIds.Contains(m.Id)) && (m.IsAlwaysAccepted))
                        .ToListAsync();
                    var manaulAC = await Context.Members
                        .Where(m => (purchaseInfoDto.MembersIds.Contains(m.Id)) && (!m.IsAlwaysAccepted))
                        .ToListAsync();
                    var buyer = await Context.Members
                        .Where(m => (m.Id == purchaseInfoDto.ByerId))
                        .SingleOrDefaultAsync();


                    PurchaseInfoDto purchaseDto = new PurchaseInfoDto
                    {
                        Id = purchase.Id,
                        ByerId = purchaseInfoDto.ByerId,
                        GroupId = purchaseInfoDto.GroupId,
                        ItemId = purchase.ItemFk,
                        MembersIds = manaulAC.Select(m=> m.Id),
                        Price = purchaseInfoDto.Price,
                    };
                    foreach (var item in autoAC)
                    {
                        var t = Context.Notifications
                            .Where(n => n.ToMemberId == item.Id)
                            .SingleOrDefault().AmountAdded;
                        item.Balance -= t;
                        buyer.Balance += t;
                        Context.Update(buyer);
                    }
                    Context.UpdateRange(autoAC);
                    await Context.SaveChangesAsync();
                    //await notificationRepository.SendMulticastAsync()
                    SendNotificationDto sendNotificationDto = await GetParameters(purchaseDto);
                    if(sendNotificationDto == null)
                    {
                        operation.OperationResultType = OperationResultTypes.Exception;
                        Transaction.Rollback();
                    }
                    else
                    {
                        var result = await notificationRepository.SendMulticastAsync(
                        sendNotificationDto.DeviceTokens,
                        sendNotificationDto.BuyerName,
                        sendNotificationDto.ItemName,
                        sendNotificationDto.GroupName,
                        set
                        );
                        if (!result)
                        {
                            operation.OperationResultType = OperationResultTypes.Failed;
                            Transaction.Rollback();
                        }
                        else
                        {
                            operation.OperationResultType = OperationResultTypes.Success;
                            Transaction.Commit();
                            operation.Result = purchaseDto;
                        }
                    }
                }
                catch (Exception ex)
                {
                    operation.OperationResultType = OperationResultTypes.Exception;
                    operation.Exception = ex;
                    Transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<bool>> RemovePurchase(int purchaseId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {

            }
            catch (Exception)
            {
                
            }
            return operation;
        }

        public async Task<OperationResult<PurchaseInfoDto>> UpdatePurchase(PurchaseInfoDto purchaseInfoDto)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<BalanceDto>> WhenAcceptPurchase(int memberId, int purchaseId)
        {
            OperationResult<BalanceDto> operation = new OperationResult<BalanceDto>();
            try
            {
                var notification = await Context.Notifications
                    .Where(n => (!n.DateDeleted.HasValue) && (n.PurchaseFK == purchaseId) && (n.ToMemberId == memberId))
                    .SingleOrDefaultAsync();
                var member = await Context.Members
                    .Where(m => (!m.DateDeleted.HasValue) && (m.Id == memberId))
                    .SingleOrDefaultAsync();
                var Buyer = await Context.Members
                    .Where(m => (!m.DateDeleted.HasValue) && (m.Id == notification.FromMemberId))
                    .SingleOrDefaultAsync();

                if (notification == null || member == null || Buyer == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                }
                else
                {
                    notification.State = 1;
                    Context.Notifications.Update(notification);
                    Buyer.Balance += notification.AmountAdded;
                    member.Balance -= notification.AmountAdded;
                    Context.Members.Update(Buyer);
                    Context.Members.Update(member);
                    await Context.SaveChangesAsync();

                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = new BalanceDto
                    {
                        MemberId = memberId,
                        Balance = member.Balance
                    };
                }
            }
            catch (Exception ex)
            {
                operation.OperationResultType = OperationResultTypes.Exception;
                operation.Exception = ex;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> WhenRefusalPurchase(int memberId, int purchaseId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var notification = await Context.Notifications
                    .Where(n => (!n.DateDeleted.HasValue) && (n.PurchaseFK == purchaseId) && (n.ToMemberId == memberId))
                    .SingleOrDefaultAsync();

                if (notification == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
                else
                {
                    notification.State = 2;
                    Context.Notifications.Update(notification);
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

        private async Task<OperationResult<FromToMoneyDto>> Calculator(List<BalanceDto> balanceDtos)
        {
            OperationResult<FromToMoneyDto> operation = new OperationResult<FromToMoneyDto>();
            try
            {
                List<FromToMoneyDto> fromToMoneyDtos = new List<FromToMoneyDto>();
                balanceDtos = balanceDtos.OrderBy(b => b.Balance).ToList();
                int i = 0 , j = balanceDtos.Count() - 1;
                while(i < j)
                {

                    if (balanceDtos[i].Balance + balanceDtos[j].Balance < 0)
                    {
                        if (balanceDtos[j].Balance == 0)
                        {
                            --j;
                            continue;
                        }

                        fromToMoneyDtos.Add(new FromToMoneyDto
                        {
                            FromUser = balanceDtos[i].Name,
                            ToUser = balanceDtos[j].Name,
                            Quantity = balanceDtos[j].Balance
                        });
                        balanceDtos[i].Balance += balanceDtos[j].Balance;
                        --j;
                    }
                    else if(balanceDtos[i].Balance + balanceDtos[j].Balance > 0)
                    {
                        if (balanceDtos[i].Balance == 0)
                        {
                            i++;
                            continue;
                        }

                        fromToMoneyDtos.Add(new FromToMoneyDto
                        {
                            FromUser = balanceDtos[i].Name,
                            ToUser = balanceDtos[j].Name,
                            Quantity = -1 * balanceDtos[i].Balance
                        });
                        balanceDtos[j].Balance += balanceDtos[i].Balance;
                        ++i;
                    }
                    else
                    {
                        if (balanceDtos[i].Balance == 0)
                        {
                            i++;
                            --j;
                            continue;
                        }

                        fromToMoneyDtos.Add(new FromToMoneyDto
                        {
                            FromUser = balanceDtos[i].Name,
                            ToUser = balanceDtos[j].Name,
                            Quantity = balanceDtos[j].Balance
                        });
                        ++i;
                        --j;
                    }
                }

                operation.IEnumerableResult = fromToMoneyDtos;
                operation.OperationResultType = OperationResultTypes.Success;
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }

        public async Task<SendNotificationDto> GetParameters(PurchaseInfoDto purchaseInfoDto)
        {
            SendNotificationDto notificationDto = new SendNotificationDto();
            try
            {
                //Get DeviceTokens
                List<Member> members = await Context.Members.Where(m => purchaseInfoDto.MembersIds
                .Contains(m.Id)).ToListAsync();
                List<int> userIds = new List<int>();
                foreach (var member in members)
                {
                    userIds.Add(member.ProfileFk);
                }
                List<string> tokens = new List<string>();
                List<Profile> profiles = await Context.Profiles
                    .Where(p => userIds.Contains(p.Id)).ToListAsync();
                foreach (var pro in profiles)
                {
                    notificationDto.DeviceTokens.Add(pro.Devices.LastOrDefault().DeviceToken);
                }

                //Get ByerName
                var user = Context.Profiles.Where(p =>
                Context.Members.Where(m => m.Id == purchaseInfoDto.ByerId)
                .SingleOrDefault().ProfileFk == p.Id).SingleOrDefault();
                notificationDto.BuyerName = user.FirstName + " " + user.LastName;

                //Get GroupName
                notificationDto.GroupName = Context.Groups.Where(g => purchaseInfoDto.GroupId == g.Id)
                    .SingleOrDefault().Name;

                //Get ItemName
                notificationDto.ItemName = Context.Items.Where(i => purchaseInfoDto.ItemId == i.Id)
                    .SingleOrDefault().Name;

                return notificationDto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OperationResult<FromToMoneyDto>> Reckoning(int groupId)
        {
            OperationResult<FromToMoneyDto> operation = new OperationResult<FromToMoneyDto>();
            using (var Transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    bool allowed = await IsReckoningAllowed(groupId);
                    if (allowed)
                    {
                        var members = await memberRepository.GetAllMemberInGroup(groupId);
                        List<BalanceDto> balanceDtos = members.IEnumerableResult.Select(u => new BalanceDto
                        {
                            MemberId = u.Id,
                            Balance = u.Balance,
                            Name = u.FirstName + " " + u.LastName
                        }).ToList();
                        await memberRepository.SetAllBalancesZero(groupId);
                        operation.IEnumerableResult = (await Calculator(balanceDtos)).IEnumerableResult;
                        operation.OperationResultType = OperationResultTypes.Success;
                        Transaction.Commit();
                    }
                    else
                    {
                        operation.OperationResultType = OperationResultTypes.Forbidden;
                        Transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    operation.OperationResultType = OperationResultTypes.Exception;
                    operation.Exception = ex;
                    Transaction.Rollback();
                }
            }
            return operation;
        }
        private async Task<bool> IsReckoningAllowed(int groupId)
        {
            var state1 = await memberRepository.GetIgnoredNotifications(groupId);
            var state2 = await memberRepository.GetRefusedNotifications(groupId);
            if(state1.IEnumerableResult == null && state2.IEnumerableResult == null)
            {
                return true;
            }
            return false;
        }
    }
}
