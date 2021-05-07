using System;

namespace VirtualBank.Api.Exceptions
{
    public class BadRequestException : Exception
    {
        public string Target { get; }

        public BadRequestException(string message, string target) : base(message) => Target = target;
        
    }
}
