using System;

namespace SqlInjection.Prevention.IO.Borker
{
    public class InvalidArgumentsException : Exception
    {
        public InvalidArgumentsException() : base() { }
        public InvalidArgumentsException(string message) : base(message) { }
    }
}