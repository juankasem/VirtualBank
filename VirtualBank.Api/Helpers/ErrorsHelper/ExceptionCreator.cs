using System;
using Microsoft.AspNetCore.Http;
using VirtualBank.Core.ApiResponseModels;

namespace VirtualBank.Api.Helpers.ErrorsHelper
{
    public static class ExceptionCreator
    {

       public static ErrorResponse CreateNotFoundError(string fieldName, string message = null) =>
           new(StatusCodes.Status400BadRequest, fieldName, MessageCreator.AddNotFound(fieldName, message));


        public static ErrorResponse CreateUnauthorizedError(string fieldName, string message = null) =>
            new(StatusCodes.Status401Unauthorized, fieldName, MessageCreator.AddUnauthorized(fieldName, message));


        public static ErrorResponse CreateBadRequestError(string fieldName, string message = null) =>
            new(StatusCodes.Status400BadRequest, fieldName, MessageCreator.AddBadRequest(fieldName, message));


        public static ErrorResponse CreateUnprocessableEntityError(string fieldName, string message = null) =>
               new(StatusCodes.Status422UnprocessableEntity, fieldName, MessageCreator.AddUnprocessableEntity(fieldName, message));


        public static ErrorResponse CreateForbiddenError() =>
                new(StatusCodes.Status401Unauthorized, "forbidden", MessageCreator.AddForbidden());


        public static ErrorResponse CreateInternalServerError() =>
                new(StatusCodes.Status500InternalServerError, "Server", MessageCreator.AddInternalServerError());


        public static ErrorResponse CreateUnexpectedError(int errorStatusCode, string description) =>
                new(errorStatusCode, "unexpected", MessageCreator.AddInternalServerError());
    }
}
