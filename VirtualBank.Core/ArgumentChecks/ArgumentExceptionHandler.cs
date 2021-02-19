using System;

namespace VirtualBank.Core.ArgumentChecks
{
    public class ArgumentExceptionHandler
    {
        public ArgumentExceptionHandler()
        {
        }

        public void If(bool condition, string argumentName, Func<string> exceptionMessage)
        {
            if (condition)
                throw new ArgumentException(exceptionMessage(), argumentName);
        }

        public void IfNot(bool condition, string argumentName, Func<string> exceptionMessage)
        {
            if (condition)
                throw new ArgumentException(exceptionMessage(), argumentName);
        }

        public string IfNullOrWhiteSpace(
          string value,
          string argumentName,
          Func<string> exceptionMessage = null)
            {
                this.If(string.IsNullOrWhiteSpace(value), argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => argumentName + " can not be null or whitespace."));
                return value;
            }

        public TValue IfDefault<TValue>(
            TValue value,
            string argumentName,
            Func<string> exceptionMessage = null)
        {
            this.If(value.Equals((object)default(TValue)), argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => argumentName + " can not be default value."));
            return value;
        }

        public DateTime IfLocalOrUnspecified(
           DateTime value,
           string argumentName,
           Func<string> exceptionMessage = null)
        {
            this.If(value.Kind != DateTimeKind.Utc, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => argumentName + " must be UTC datetime kind."));
            return value;
        }

    }
}
