using System;
namespace VirtualBank.Core.ApiModels
{
    public class CheckEmailResponse
    {
        /// <summary>
        /// whether the email already registered or not
        /// </summary>
        public bool Exists { get; set; }
    }
}
