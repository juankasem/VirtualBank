using System;
using System.Collections.Generic;

namespace VirtualBank.Core.ApiResponseModels
{
    public class ApiResponse
    {
        public bool Success => Errors == null;

        public List<string> Errors { get; private set; }

        public void AddError(string error)
        {
            if(Errors == null)
                Errors = new List<string>();

            Errors.Add(error);

        }

        public void AddErrors(List<string> errors)
        {
            if (Errors == null)
                Errors = new List<string>();

            Errors.AddRange(errors);

        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
}




