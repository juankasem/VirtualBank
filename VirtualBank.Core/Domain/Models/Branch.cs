using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class Branch
    {
        public int Id { get; }

        public string Code { get; }

        public string Name { get; }

        public string Phone { get; }

        public Address Address { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public Branch(int id, string code, string name, string phone, Address address,
                      CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Code = Throw.ArgumentException.IfDefault(code, nameof(code));
            Name = Throw.ArgumentException.IfDefault(name, nameof(name));
            Phone = Throw.ArgumentException.IfDefault(phone, nameof(phone));
            Address = Throw.ArgumentException.IfDefault(address, nameof(address));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}