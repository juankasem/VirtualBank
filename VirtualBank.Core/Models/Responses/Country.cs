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

        public Country(int id, string name, string code, CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Code = Throw.ArgumentException.IfDefault(code, nameof(code));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}