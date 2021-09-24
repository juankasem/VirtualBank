using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models
{
    public class CreationInfo
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; }

        public CreationInfo(string createdBy, DateTime createdOn)
        {
            CreatedBy = Throw.ArgumentException.IfNullOrWhiteSpace(createdBy, nameof(createdBy));
            Throw.ArgumentException.IfDefault(createdOn, nameof(createdOn));
            CreatedOn = Throw.ArgumentException.IfLocalOrUnspecified(createdOn, nameof(createdOn));
        }
    }
}
