using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IDistrictMapper
    {
        District MapToResponseModel(VirtualBank.Core.Entities.District district);

        Address.District MapToAddressDistrict(VirtualBank.Core.Entities.District district);
    }

    public class DistrictMapper : IDistrictMapper
    {
        private readonly ICityMapper _cityMapper;

        public DistrictMapper(ICityMapper cityMapper)
        {
            _cityMapper = cityMapper;
        }

        public District MapToResponseModel(Core.Entities.District district) =>
       new(
           district.Id,
           district.Name,
           _cityMapper.MapToResponseModel(district.City),
            CreateCreationInfo(district.CreatedBy, district.CreatedOn),
           CreateModificationInfo(district.LastModifiedBy, district.LastModifiedOn)
       );

        public Address.District MapToAddressDistrict(Core.Entities.District district) =>
       new(
            district.Id,
            district.Name,
            district.CityId
        );

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) => new(lastModifiedBy, lastModifiedOn);
    }
}