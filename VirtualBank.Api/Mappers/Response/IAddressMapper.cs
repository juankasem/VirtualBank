using VirtualBank.Core.Entities;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IAddressMapper
    {
        Address MapToResponseModel(VirtualBank.Core.Entities.Address address);
    }

    public class AddressMapper : IAddressMapper
    {
        public Address MapToResponseModel(Core.Entities.Address address)
        {
            throw new System.NotImplementedException();
        }
    }
}