using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace UwCore.Common
{
    public static class Guard
    {
        [DebuggerStepThrough]
        public static void NotNull([CanBeNull]object argument, [NotNull]string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        [DebuggerStepThrough]
        public static void NotNullOrWhiteSpace([CanBeNull]string argument, [NotNull]string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (string.IsNullOrWhiteSpace(argumentName))
                throw new ArgumentException("String is whitespace.", argumentName);
        }

        [DebuggerStepThrough]
        public static void NotNullOrEmpty([CanBeNull]IEnumerable argument, [NotNull]string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (argument.GetEnumerator().MoveNext() == false)
                throw new ArgumentException("List is empty.", argumentName);
        }

        [DebuggerStepThrough]
        public static void NotInvalidEnum([CanBeNull]object argument, [NotNull]string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (argument.GetType().GetTypeInfo().IsEnum == false)
                throw new InvalidOperationException("The NotInvalidEnum only works with enum values.");

            if (Enum.IsDefined(argument.GetType(), argument) == false)
                throw new ArgumentException("Unknown enum value.", argumentName);
        }

        [DebuggerStepThrough]
        public static void NotZeroOrNegative(long argument, [NotNull]string argumentName)
        {
            if (argument <= 0)
                throw new ArgumentException("Value is equal or less than zero.", argumentName);
        }

        [DebuggerStepThrough]
        public static void NotZeroOrNegative(TimeSpan argument, [NotNull]string argumentName)
        {
            if (argument < TimeSpan.Zero)
                throw new ArgumentException("Value is equal or less than zero.", argumentName);
        }

        [DebuggerStepThrough]
        public static void NotInvalidDateTime([CanBeNull]DateTime? argument, [NotNull]string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (argument <= new DateTime(1900, 1, 1))
                throw new ArgumentException("Value is before 1900.", argumentName);
        }
    }
}