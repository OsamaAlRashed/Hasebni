using Hasebni.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hasebni.Dashboard.Services.Interfaces
{
    public interface IGroupHttpRepository
    {
        Task<List<GroupModel>> GetGroups();
        Task<List<GroupModel>> GetGroupsForUser(int userId);
    }
}
