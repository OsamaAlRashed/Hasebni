using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hasebni.Base;
using Hasebni.Main.Data.Repository;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.Security.Idata.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Hasebni.API.Pages;

namespace Hasebni.API.Controllers
{
   // [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IAPIRepository APIRepository;
        private readonly IGroupRepository groupRepository;

        public IHubContext<TestSignalR> Hub { get; }

        public DashboardController(IAPIRepository APIRepository
            ,IGroupRepository groupRepository
            ,IHubContext<TestSignalR> hub)
        {
            this.APIRepository = APIRepository;
            this.groupRepository = groupRepository;
            Hub = hub;
        }

        //Dashboard/GetAllUsers
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await APIRepository.GetUsers();
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.IEnumerableResult) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Dashboard/GetAllGroupsForUser
        [HttpGet]
        public async Task<IActionResult> GetAllGroupsForUser(int userId)
        {
            var result = await groupRepository.GetAllGroupsForUser(userId);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.IEnumerableResult) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        //Dashboard/GetAllGroups
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var result = await groupRepository.GetAllGroups();
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.IEnumerableResult) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }


    }
}
