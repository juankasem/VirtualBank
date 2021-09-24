using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
{
    public class Address
    {
        public int Id { get; }
        public string Name { get; }
        public string Street { get; }
        public string PostalCode { get; }
        public District District { get; }
        public City City { get; }
        public Country Country { get; }
        public CreationInfo CreationInfo { get; }
        public ModificationInfo ModificationInfo { get; }

        public Address(int id, string name, string street, string postalCode,
                       District district, City city, Country country,
                       CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Street = Throw.ArgumentException.IfDefault(street, nameof(street));
            PostalCode = Throw.ArgumentException.IfDefault(postalCode, nameof(postalCode));
            District = Throw.ArgumentException.IfDefault(district, nameof(district));
            City = Throw.ArgumentException.IfDefault(city, nameof(city));
            Country = Throw.ArgumentException.IfDefault(country, nameof(country));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}