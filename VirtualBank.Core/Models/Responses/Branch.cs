using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
{
    public class Branch
    {
        public int Id { get; }
        public string Code { get; }
        public string Name { get; }
        public Address Address { get; }
        public CreationInfo CreationInfo { get; }
        public ModificationInfo ModificationInfo { get; }

        public Branch(int id, string code, string name, Address address,
                      CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Code = Throw.ArgumentException.IfDefault(code, nameof(code));
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Address = Throw.ArgumentException.IfDefault(address, nameof(address));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}