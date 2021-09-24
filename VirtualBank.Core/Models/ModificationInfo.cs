using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models
{
    public class ModificationInfo
    {
        public string ModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; }

        public ModificationInfo(string modifiedBy, DateTime lastModifiedOn)
        {
            ModifiedBy = Throw.ArgumentException.IfNullOrWhiteSpace(modifiedBy, nameof(modifiedBy));
            Throw.ArgumentException.IfDefault(lastModifiedOn, nameof(lastModifiedOn));
            LastModifiedOn = Throw.ArgumentException.IfLocalOrUnspecified(lastModifiedOn, nameof(lastModifiedOn));
        }
    }
}