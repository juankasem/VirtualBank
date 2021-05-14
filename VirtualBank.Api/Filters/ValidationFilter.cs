using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtualBank.Core.ApiResponseModels;

namespace VirtualBank.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
     
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                var apiResponse = new ApiResponse();

                foreach (var errorKeyValue in errorsInModelState)
                {
                    foreach (var error in errorKeyValue.Value)
                    {
                        var errorResponse = new ErrorResponse()
                        {
                            Code = StatusCodes.Status400BadRequest,
                            FieldName = errorKeyValue.Key,
                            Message = error
                        };

                        apiResponse.AddError(errorResponse);
                    }
                }

                context.Result = new BadRequestObjectResult(apiResponse);
                return;
            }

            await next();
        }
    }
}
