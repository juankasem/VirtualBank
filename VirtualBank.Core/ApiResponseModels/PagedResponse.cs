using System;
using System.Collections.Generic;

namespace VirtualBank.Core.ApiResponseModels
{
    public class PagedResponse<T>
    {
        public PagedResponse()
        {
        }

        public PagedResponse(T data) 
        {
            Data = data;
        }

        public T Data { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public string NextPage { get; set; }

        public string PreviousPage { get; set; }

    }
}
