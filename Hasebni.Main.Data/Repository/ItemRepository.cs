using Hasebni.Base;
using Hasebni.Main.Dto.Item;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.Model.Main;
using Hasebni.SqlServer.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Main.Data.Repository
{
    public class ItemRepository : HasebniRepository , IItemRepository
    {
        public ItemRepository(HasebniDbContext context):base(context)
        {

        }

        public async Task<OperationResult<ItemInfoDto>> AddItem(ItemInfoDto itemInfoDto)
        {
            OperationResult<ItemInfoDto> operation = new OperationResult<ItemInfoDto>();
            try
            {
                var item = new Item
                {
                    Name = itemInfoDto.Name,
                    GroupFk = itemInfoDto.GroupId
                };
                Context.Items.Add(item);
                await Context.SaveChangesAsync();

                operation.OperationResultType = OperationResultTypes.Success;
                operation.Result = new ItemInfoDto
                {
                    Id = item.Id,
                    GroupId = item.GroupFk,
                    Name = item.Name
                };
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }

        public async Task<OperationResult<ItemInfoDto>> GetItems(int groupId)
        {
            OperationResult<ItemInfoDto> operation = new OperationResult<ItemInfoDto>();
            try
            {
                var items = await Context.Items
                    .Where(i => (!i.DateDeleted.HasValue))
                    .Select(i => new ItemInfoDto
                    {
                        Id = i.Id,
                        GroupId = i.GroupFk,
                        Name = i.Name
                    }).ToListAsync();
                operation.IEnumerableResult = items;
                operation.OperationResultType = OperationResultTypes.Success;
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }

        public async Task<OperationResult<bool>> RemoveItem(int itemId)
        {
            OperationResult<bool> operation = new OperationResult<bool>();
            try
            {
                var item = await Context.Items
                    .Where(i => (!i.DateDeleted.HasValue) && (i.Id == itemId))
                    .SingleOrDefaultAsync();
                if(item == null)
                {
                    operation.OperationResultType = OperationResultTypes.NotExist;
                    operation.Result = false;
                }
                else
                {
                    item.DateDeleted = DateTimeOffset.UtcNow;
                    await Context.SaveChangesAsync();
                    operation.OperationResultType = OperationResultTypes.Success;
                    operation.Result = true;
                }
            }
            catch (Exception ex)
            {
                operation.Exception = ex;
                operation.OperationResultType = OperationResultTypes.Exception;
            }
            return operation;
        }
    }
}
