namespace SqlInjection.Prevention.IO.Borker
{
    using System;
    public class InvalidArgumentsException : Exception
    {
        public InvalidArgumentsException() : base() { }
        public InvalidArgumentsException(string message) : base(message) { }
    }
}