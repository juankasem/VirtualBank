using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
{
    public class Address
    {
        public int Id { get; }

        public string Name { get; }

        public string Street { get; }

        public string PostalCode { get; }

        public District AddressDistrict { get; }

        public City AddressCity { get; }

        public Country AddressCountry { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }

        public Address(int id, string name, string street, string postalCode,
                       District addressDistrict, City addressCity, Country addressCountry,
                       CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Street = Throw.ArgumentException.IfDefault(street, nameof(street));
            PostalCode = Throw.ArgumentException.IfDefault(postalCode, nameof(postalCode));
            AddressDistrict = Throw.ArgumentException.IfDefault(addressDistrict, nameof(addressDistrict));
            AddressCity = Throw.ArgumentException.IfDefault(addressCity, nameof(addressCity));
            AddressCountry = Throw.ArgumentException.IfDefault(addressCountry, nameof(addressCountry));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }

        public class City
        {
            public int Id { get; }

            public string Name { get; }

            public int CountryId { get; }

            public City(int id, string name, int countryId)
            {
                Id = id;
                Name = Throw.ArgumentException.IfDefault(name, nameof(name));
                CountryId = countryId;
            }
        }

        public class Country
        {
            public int Id { get; }
            public string Name { get; }
            public string Code { get; }

            public Country(int id, string name, string code)
            {
                Id = id;
                Name = Throw.ArgumentException.IfDefault(name, nameof(name));
                Code = Throw.ArgumentException.IfDefault(code, nameof(code));
            }
        }
        public class District
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public int CityId { get; set; }

            public District(int id, string name, int cityId)
            {
                Id = id;
                Name = Throw.ArgumentException.IfDefault(name, nameof(name));
                CityId = cityId;
            }
        }
    }
}