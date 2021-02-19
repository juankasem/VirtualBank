using System;
namespace VirtualBank.Core.ArgumentChecks
{
    public sealed class ArgumentNullExceptionHandler
    {

        internal ArgumentNullExceptionHandler()
        {

        }

        public void If(bool condition, string argumentName, Func<string> exceptionMessage)
        {
            if (condition)
                throw new ArgumentNullException(argumentName, exceptionMessage());
        }

        public void IfNot(bool condition, string argumentName, Func<string> exceptionMessage)
        {
            if (!condition)
                throw new ArgumentNullException(exceptionMessage(), argumentName);
        }

        public TValue IfNull<TValue>(TValue value, string argumentName, Func<string> exceptionMessage = null)
        {
            this.If((object)value == null, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => argumentName + " can not be null."));
            return value;
        }
    }
}
