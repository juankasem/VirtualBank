using System;
using System.Collections.Generic;

namespace VirtualBank.Core.ApiResponseModels
{
    public class ApiResponse
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
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
}




