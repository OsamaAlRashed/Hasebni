using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hasebni.Base;
using Hasebni.Main.Dto.Member;
using Hasebni.Main.Idata.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hasebni.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberRepository memberRepository;

        public MemberController(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddMembers(int id , IEnumerable<string> users)
        {
            var result = await memberRepository.AddMembers(id,users);
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

        [HttpDelete]
        public async Task<IActionResult> DeleteMember(int memberId)
        {
            var result = await memberRepository.DeleteMember(memberId);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
                case OperationResultTypes.NotExist:
                    return new JsonResult("Not Exist") { StatusCode = 204 };
                case OperationResultTypes.Forbidden:
                    return new JsonResult("Forbidden") { StatusCode = 403 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMemberInGroup(int groupId)
        {
            var result = await memberRepository.GetAllMemberInGroup(groupId);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        [HttpGet]
        public async Task<IActionResult> IsAllBalancesZero(int groupId)
        {
            var result = await memberRepository.IsAllBalancesZero(groupId);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        [HttpPut]
        public async Task<IActionResult> ChangeAdmin(ChangeAdminDto changeAdminDto)
        {
            var result = await memberRepository.ChangeAdmin(changeAdminDto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.Result) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }






    }
}
