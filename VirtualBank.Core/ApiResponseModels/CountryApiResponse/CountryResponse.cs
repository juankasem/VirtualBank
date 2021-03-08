using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountryResponse
    {
        public string Name { get; }

        public string Code { get; }

        public CountryResponse(string name, string code)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
        }
    }
}
