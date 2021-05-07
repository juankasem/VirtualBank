using System;

namespace VirtualBank.Api.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message): base(message)
        {
        }
    }
}
