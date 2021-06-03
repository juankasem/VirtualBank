using System;
using System.Collections.Generic;

namespace VirtualBank.Core.ApiResponseModels
{
    public class Response
    {
        public bool Success => Errors == null;

        public List<ErrorResponse> Errors { get; private set; }

        public void AddError(ErrorResponse error)
        {
            if(Errors == null)
                Errors = new List<ErrorResponse>();

            Errors.Add(error);
        }

        public void AddErrors(List<ErrorResponse> errors)
        {
            if (Errors == null)
                Errors = new List<ErrorResponse>();

            Errors.AddRange(errors);
        }

        public string Message { get; set; }
    }

    public class ApiResponse<T> : Response
    {
        public T Data { get; set; }
    }
}




