using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class District
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public City City { get; set; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public District(int id, string name, City city, CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            City = Throw.ArgumentException.IfDefault(city, nameof(city));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}