using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Exceptions
{
    public class AgeLimitException : Exception
    {
        public AgeLimitException(string notification) : base(notification)
        {

        }
    }
}
