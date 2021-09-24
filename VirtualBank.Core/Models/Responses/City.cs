using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
{
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
}