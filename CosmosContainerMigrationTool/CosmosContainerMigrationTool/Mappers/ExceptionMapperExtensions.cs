using CosmosContainerMigrationTool.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CosmosContainerMigrationTool.Mappers
{
    public static class ExceptionMapperExtensions
    {
        public static ObjectResult ToResult(this Exception ex, int statusCode)
        {
            ObjectResult result = new ObjectResult(new FailureResult
            {
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace
            })
            {
                StatusCode = statusCode
            };

            return result;
        }
    }
}
