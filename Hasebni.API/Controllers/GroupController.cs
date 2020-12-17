using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hasebni.Base;
using Hasebni.Main.Dto;
using Hasebni.Main.Idata.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hasebni.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository groupRepository;
        private readonly IMemberRepository memberRepository;

        public GroupController(IGroupRepository groupRepository
            ,IMemberRepository memberRepository)
        {
            this.groupRepository = groupRepository;
            this.memberRepository = memberRepository;
        }

        //Group/CreateGroup
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm]CreateGroupDto createGroupDto)
        {
            string myurl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            var result = await groupRepository.CreateGroup(createGroupDto, myurl);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }


        //Group/UpdateGroup
        [HttpPut]
        public async Task<IActionResult> UpdateGroup([FromForm]GroupInfoDto GroupInfoDto)
        {
            string myurl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            var result = await groupRepository.UpdateGroup(GroupInfoDto , myurl);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("Not Exist") { StatusCode = 204 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }


        //Group/DeleteGroup
        [HttpDelete]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            
            var result = await groupRepository.DeleteGroup(id);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("Not Exist") { StatusCode = 204 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }


        //Group/SearchGroups
        [HttpGet]
        public async Task<IActionResult> SearchGroups(string text)
        {

            var result = await groupRepository.SearchGroups(text);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }





    }
}
