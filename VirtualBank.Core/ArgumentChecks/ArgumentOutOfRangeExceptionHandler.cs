using System;
namespace VirtualBank.Core.ArgumentChecks
{
    public class ArgumentOutOfRangeExceptionHandler
    {
        public void If(bool condition, string argumentName, Func<string> exceptionMessage)
        {
            if (condition)
                throw new ArgumentOutOfRangeException(argumentName, exceptionMessage());
        }

        public void IfNot(bool condition, string argumentName, Func<string> exceptionMessage)
        {
            if (!condition)
                throw new ArgumentOutOfRangeException(exceptionMessage(), argumentName);
        }

        public TArg IfLessThan<TArg>(
          TArg value,
          TArg condition,
          string argumentName,
          Func<string> exceptionMessage = null)
          where TArg : IComparable
        {
            this.If(value.CompareTo((object)condition) < 0, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => string.Format("{0} can not be less than {1}. Argument '{2}'", new object[3]
          {
        (object) argumentName,
        (object) (TArg) condition,
        (object) (TArg) value
          })));
            return value;
        }

        public TArg IfLessThanOrEqualTo<TArg>(
          TArg value,
          TArg condition,
          string argumentName,
          Func<string> exceptionMessage = null)
          where TArg : IComparable
        {
            this.If(value.CompareTo((object)condition) <= 0, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => string.Format("{0} can not be less than or equal to {1}. Argument '{2}'", new object[3]
          {
        (object) argumentName,
        (object) (TArg) condition,
        (object) (TArg) value
          })));
            return value;
        }

        public TArg IfGreaterThan<TArg>(
          TArg value,
          TArg condition,
          string argumentName,
          Func<string> exceptionMessage = null)
          where TArg : IComparable
        {
            this.If(value.CompareTo((object)condition) > 0, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => string.Format("{0} can not be greater than {1}. Argument '{2}'", new object[3]
          {
        (object) argumentName,
        (object) (TArg) condition,
        (object) (TArg) value
          })));
            return value;
        }

        public TArg IfGreaterThanOrEqualTo<TArg>(
          TArg value,
          TArg condition,
          string argumentName,
          Func<string> exceptionMessage = null)
          where TArg : IComparable
        {
            this.If(value.CompareTo((object)condition) >= 0, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => string.Format("{0} can not be greater than or equal to {1}. Argument '{2}'", new object[3]
          {
        (object) argumentName,
        (object) (TArg) condition,
        (object) (TArg) value
          })));
            return value;
        }

        public TArg IfBetween<TArg>(
          TArg value,
          TArg lower,
          TArg upper,
          string argumentName,
          Func<string> exceptionMessage = null)
          where TArg : IComparable
        {
            this.If(value.CompareTo((object)lower) >= 0 && value.CompareTo((object)upper) <= 0, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => string.Format("{0} must not be between {1} and {2}. Argument '{3}'", (object)argumentName, (object)(TArg)lower, (object)(TArg)upper, (object)(TArg)value)));
            return value;
        }

        public TArg IfNotBetween<TArg>(
          TArg value,
          TArg lower,
          TArg upper,
          string argumentName,
          Func<string> exceptionMessage = null)
          where TArg : IComparable
        {
            this.IfNot(value.CompareTo((object)lower) >= 0 && value.CompareTo((object)upper) <= 0, argumentName, exceptionMessage != null ? exceptionMessage : (Func<string>)(() => string.Format("{0} must be between {1} and {2}. Argument '{3}'", (object)argumentName, (object)(TArg)lower, (object)(TArg)upper, (object)(TArg)value)));
            return value;
        }
    }
}
