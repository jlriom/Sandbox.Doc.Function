using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosContainerMigrationTool.Implementations.CosmosBulkOperations
{
    public static class OperationHandler
    {
        public static async Task<OperationResponse<T>> CaptureOperationResponse<T>(Task<ItemResponse<T>> task, T item)
        {
            try
            {
                ItemResponse<T> response = await task;
                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = true,
                    RequestUnitsConsumed = task.Result.RequestCharge
                };
            }
            catch (Exception ex)
            {
                if (ex is CosmosException cosmosException)
                {
                    return new OperationResponse<T>()
                    {
                        Item = item,
                        RequestUnitsConsumed = cosmosException.RequestCharge,
                        IsSuccessful = false,
                        CosmosException = cosmosException
                    };
                }

                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessful = false,
                    CosmosException = ex
                };
            }
        }
    }
}
