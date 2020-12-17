using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hasebni.Base;
using Hasebni.Main.Dto.Purchase;
using Hasebni.Main.Idata.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hasebni.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseRepository purchaseRepository;

        public PurchaseController(IPurchaseRepository purchaseRepository)
        {
            this.purchaseRepository = purchaseRepository;
        }

        //[HttpPost]
        //public async Task<IActionResult> Calculator(List<BalanceDto> balanceDtos)
        //{
        //    var result = await purchaseRepository.Calculator(balanceDtos);
        //    switch (result.OperationResultType)
        //    {
        //        case OperationResultTypes.Exception:
        //            return new JsonResult("Exception") { StatusCode = 400 };
        //        case OperationResultTypes.Success:
        //            return new JsonResult(result.IEnumerableResult) { StatusCode = 200 };
        //    }
        //    return new JsonResult("Unknown Error") { StatusCode = 500 };
        //}

        [HttpPost]
        public async Task<IActionResult> Reckoning(int groupId)
        {
            var result = await purchaseRepository.Reckoning(groupId);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.IEnumerableResult) { StatusCode = 200 };
                case OperationResultTypes.Forbidden:
                    return new JsonResult("Forbidden") { StatusCode = 403 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchase(PurchaseInfoDto purchaseInfoDto)
        {
            var result = await purchaseRepository.AddPurchase(purchaseInfoDto);
            switch (result.OperationResultType)
            {
                case OperationResultTypes.Exception:
                    return new JsonResult("Exception") { StatusCode = 400 };
                case OperationResultTypes.Failed:
                    return new JsonResult("Failed") { StatusCode = 404 };
                case OperationResultTypes.Success:
                    return new JsonResult(result.IEnumerableResult) { StatusCode = 200 };
            }
            return new JsonResult("Unknown Error") { StatusCode = 500 };
        }
    }
}
