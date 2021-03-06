﻿using System;
namespace VirtualBank.Core.ArgumentChecks
{
    public static class Throw
    {
        public static ArgumentExceptionHandler ArgumentException => new ArgumentExceptionHandler();


        public static ArgumentNullExceptionHandler ArgumentNullException => new ArgumentNullExceptionHandler();


        public static ArgumentOutOfRangeExceptionHandler ArgumentOutOfRangeException => new ArgumentOutOfRangeExceptionHandler();


        public static InvalidOperationExceptionHandler InvalidOperationException => new InvalidOperationExceptionHandler();
    }
}
