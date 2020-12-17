using Hasebni.Base;
using Hasebni.Main.Dto.Item;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Main.Idata.Interfaces
{
    public interface IItemRepository
    {
        Task<OperationResult<ItemInfoDto>> AddItem(ItemInfoDto itemInfoDto);
        Task<OperationResult<bool>> RemoveItem(int itemId);
        Task<OperationResult<ItemInfoDto>> GetItems(int groupId);

        

    }
}
