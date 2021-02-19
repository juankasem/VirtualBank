using System;
namespace VirtualBank.Core.ArgumentChecks
{
    public class InvalidOperationExceptionHandler
    {
        public void If(bool condition, Func<string> exceptionMessage, Exception inner = null)
        {
            if (!condition)
                return;
            if (inner != null)
                throw new InvalidOperationException(exceptionMessage(), inner);
            throw new InvalidOperationException(exceptionMessage());
        }

        public void IfNot(bool condition, Func<string> exceptionMessage, Exception inner = null)
        {
            if (condition)
                return;
            if (inner != null)
                throw new InvalidOperationException(exceptionMessage(), inner);
            throw new InvalidOperationException(exceptionMessage());
        }
    }
}
