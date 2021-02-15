using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels
{
    public class CheckEmailRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
