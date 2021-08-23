using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models
{
    public class ModificationInfo
    {
        public DateTime ModifiedOn { get; }
        public string ModifiedBy { get; set; }

        public ModificationInfo(string modifiedBy, DateTime modifiedOn)
        {
            ModifiedBy = Throw.ArgumentException.IfNullOrWhiteSpace(modifiedBy, nameof(modifiedBy));
            Throw.ArgumentException.IfDefault(modifiedOn, nameof(modifiedOn));
            modifiedOn = Throw.ArgumentException.IfLocalOrUnspecified(modifiedOn, nameof(modifiedOn));
        }
    }
}
