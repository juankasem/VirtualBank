using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
{
    public class Country
    {
        public int Id { get; }

        public string Name { get; }

        public string Code { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }

        public ImmutableList<City> Cities { get; set; }


        public Country(int id, string name, string code, CreationInfo creationInfo, ModificationInfo modificationInfo, ImmutableList<City> cities)
        {
            Id = id;
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Code = Throw.ArgumentException.IfDefault(code, nameof(code));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
            Cities = cities.IsEmpty ? ImmutableList<City>.Empty : cities;
        }

        public class City
        {
            public int Id { get; }

            public string Name { get; }

            public City(int id, string name)
            {
                Id = id;
                Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            }
        }
    }
}