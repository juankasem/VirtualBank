﻿using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountryResponse
    {
        public int Id { get; set; }

        public string Name { get; }

        public string Code { get; }

        public CountryResponse(int id, string name, string code)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
        }
    }
}
