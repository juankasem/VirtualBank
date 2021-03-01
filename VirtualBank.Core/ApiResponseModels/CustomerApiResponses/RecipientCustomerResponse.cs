using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class RecipientCustomerResponse
    {
        public string FirstName { get; }
     
        public string LastName { get;  }

        public RecipientCustomerResponse(string firstName, string lastName)
        {
            FirstName = Throw.ArgumentNullException.IfNull(firstName, nameof(firstName));
            LastName = Throw.ArgumentNullException.IfNull(lastName, nameof(lastName));
        }
    }
}
