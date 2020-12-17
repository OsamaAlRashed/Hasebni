using Hasebni.Base;
using Hasebni.Main.Dto.Member;
using Hasebni.Main.Dto.Notification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Main.Idata.Interfaces
{
    public interface IMemberRepository
    {
        Task<OperationResult<MemberInfoDto>> AddMembers(int groupId , IEnumerable<string> users);
        Task<OperationResult<bool>> DeleteMember(int memberId);
        Task<OperationResult<bool>> ChangeAdmin(ChangeAdminDto changeAdminDto);
        Task<OperationResult<MemberInfoDto>> GetAllMemberInGroup(int id);
        Task<OperationResult<double>> GetBalanceUser(int memberId);
        Task<OperationResult<long>> GetBalancesForUsersInGroup(int groupId);
        Task<OperationResult<bool>> IsAllBalancesZero(int id);
        Task<OperationResult<bool>> LeaveGroup(int memberId);

        Task<OperationResult<NotificationInfoDto>> GetIgnoredNotifications(int groupId);
        Task<OperationResult<NotificationInfoDto>> GetRefusedNotifications(int groupId);
        Task<OperationResult<bool>> IsAdmin(int memberId);
        Task<bool> SetAllBalancesZero(int groupId);

        Task<OperationResult<bool>> IsAlwaysAccepted(int memberId, bool state);

        //Task<OperationResult<>> 
        //Task<OperationResult<IEnumerable<long>>> Calculation(IEnumerable<IEnumerable<>>)

    }
}
