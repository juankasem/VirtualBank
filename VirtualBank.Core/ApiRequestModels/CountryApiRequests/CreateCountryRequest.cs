using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.CountryApiRequests
{
    public class CreateCountryRequest
    {
        public string Name { get; set; }

        public string Code { get; set; }
        public CreationInfo CreationInfo { get; set; }

        public CreateCountryRequest(string name, string code, CreationInfo creationInfo)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
