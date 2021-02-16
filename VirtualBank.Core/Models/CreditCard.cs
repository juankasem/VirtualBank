using System;
namespace VirtualBank.Core.Models
{
    public class CreditCard
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
