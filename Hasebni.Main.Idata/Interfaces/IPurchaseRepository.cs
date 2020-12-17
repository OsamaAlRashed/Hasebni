using Hasebni.Base;
using Hasebni.Main.Dto.Purchase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Main.Idata.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<OperationResult<PurchaseInfoDto>> AddPurchase(PurchaseInfoDto purchaseInfoDto);
        Task<OperationResult<bool>> RemovePurchase(int purchaseId);
        Task<OperationResult<PurchaseInfoDto>> UpdatePurchase(PurchaseInfoDto purchaseInfoDto);
        Task<OperationResult<BalanceDto>> WhenAcceptPurchase(int memberId, int purchaseId);
        Task<OperationResult<bool>> WhenRefusalPurchase(int memberId, int purchaseId);
       // Task<OperationResult<FromToMoneyDto>> Calculator(List<BalanceDto> balanceDtos);
        Task<OperationResult<FromToMoneyDto>> Reckoning(int groupId);
       // Task<OperationResult<bool>> IsReckoningAllowed(int groupId);

    }
}
