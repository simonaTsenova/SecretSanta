using System;

namespace SecretSanta.Common.Exceptions
{
    public class AccessForbiddenException : Exception
    {
        public AccessForbiddenException()
        {
        }

        public AccessForbiddenException(string message)
            : base(message)
        {
        }
    }
}
