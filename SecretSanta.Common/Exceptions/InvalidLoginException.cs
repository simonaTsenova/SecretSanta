using System;

namespace SecretSanta.Common.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException()
        {
        }

        public InvalidLoginException(string message)
            : base(message)
        {
        }
    }
}
