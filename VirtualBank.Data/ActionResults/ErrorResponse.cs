using System;
namespace VirtualBank.Data.ActionResults
{
#nullable enable
    public class ErrorResponse
    {
        public Error Error { get; }

        public ErrorResponse(Error error)
        {
            Error = error;
        }
    }

    public class Error
    {
        public string Code { get; }

        public string Message { get; }

        public ErrorDetails[] Details { get; }


        public Error(string code, string message, ErrorDetails[]? details = null)
        {
            Code = code;
            Message = message;
            Details = details ?? new ErrorDetails[0];
        }
    }


    public class ErrorDetails
    {
        public string Target { get; }

        public ErrorMessage[] Errors { get; }

        public ErrorDetails(string target, ErrorMessage[]? errors = null)
        {
            Target = target;

            Errors = errors ?? new ErrorMessage[0];
        }
    }

    public class ErrorMessage
    {
        public string Message { get; }

        public ErrorMessage(string message)
        {
            Message = message;
        }
    }
}
