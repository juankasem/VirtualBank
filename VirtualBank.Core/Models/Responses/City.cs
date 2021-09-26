using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
{
    public class City
    {
        public int Id { get; }
        public string Name { get; }
        public Country Country { get; }
        public CreationInfo CreationInfo { get; }
        public ModificationInfo ModificationInfo { get; }

        public City(int id, string name, Country country, CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Country = Throw.ArgumentException.IfDefault(country, nameof(country));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}