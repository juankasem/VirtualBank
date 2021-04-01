using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.RecipientApiResponses
{
    public class RecipientResponse
    {
        public int Id { get;  }

        public string IBAN { get; }

        public string FullName { get; }

        public string ShortName { get; }


        public RecipientResponse(int id, string iban, string fullName, string shortName)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            ShortName = shortName;
        }
    }
}
