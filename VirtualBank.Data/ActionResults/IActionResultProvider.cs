using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace VirtualBank.Data.ActionResults
{
    public interface IActionResultProvider
    {
        IActionResult GetBadRequestErrorResponse(string errorMessage, string target);

        IActionResult GetUnprocessableEntityErrorResponse(string errorMessage);

        IActionResult GetNotfoundErrorResponse(string errorMessage);

        IActionResult GetNotModifiedResponse();

        IActionResult GetConflictErrorResponse(string errorMessage);
    }

    public class ActionResultProvider : IActionResultProvider
    {
        public IActionResult GetBadRequestErrorResponse(string errorMessage, string target) =>

            new BadRequestObjectResult(new ErrorResponse(
                new Error(
                nameof(HttpStatusCode.BadRequest),
                errorMessage, new[] { new ErrorDetails(target) })));


        public IActionResult GetUnprocessableEntityErrorResponse(string errorMessage) =>

            new ObjectResult(
                new ErrorResponse(
                    new Error(
                        nameof(HttpStatusCode.UnprocessableEntity),
                        errorMessage
                        ))
                )
            { StatusCode = (int)HttpStatusCode.UnprocessableEntity };
        

        public IActionResult GetNotfoundErrorResponse(string errorMessage) =>
        
            new ObjectResult(new ErrorResponse(
                new Error(
                    nameof(HttpStatusCode.NotFound),
                    errorMessage)))

            { StatusCode = (int)HttpStatusCode.NotFound };


        public IActionResult GetNotModifiedResponse() =>
              new StatusCodeResult((int)HttpStatusCode.NotModified);

        public IActionResult GetConflictErrorResponse(string errorMessage) =>
            new ObjectResult(new ErrorResponse(
                    new Error(
                        nameof(HttpStatusCode.Conflict),
                        errorMessage)
                ))
            { StatusCode = (int)HttpStatusCode.Conflict };
    }
}
