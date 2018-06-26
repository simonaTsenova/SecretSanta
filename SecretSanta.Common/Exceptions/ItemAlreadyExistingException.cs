using System;

namespace SecretSanta.Common.Exceptions
{
    public class ItemAlreadyExistingException : Exception
    {
        public ItemAlreadyExistingException()
        {
        }

        public ItemAlreadyExistingException(string message)
            : base(message)
        {
        }
    }
}
