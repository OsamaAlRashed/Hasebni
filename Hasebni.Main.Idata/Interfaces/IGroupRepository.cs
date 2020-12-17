using Hasebni.Base;
using Hasebni.Main.Dto;
using Hasebni.Main.Dto.Member;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Main.Idata.Interfaces
{
    public interface IGroupRepository
    {
        Task<OperationResult<int>> CreateGroup(CreateGroupDto createGroupDto , string myurl);
        Task<OperationResult<GroupInfoDtoResponse>> UpdateGroup(GroupInfoDto groupInfoDto, string myurl);
        Task<OperationResult<bool>> DeleteGroup(int id);
        Task<OperationResult<GroupInfoDtoResponse>> GetAllGroupsForUser(int userId);
        Task<OperationResult<GroupInfoDtoResponse>> GetAllGroups();
        Task<OperationResult<GroupInfoDtoResponse>> SearchGroups(string text);
    }
}
