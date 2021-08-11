using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Exceptions
{
    public class NotEnoughMoney : Exception
    {
        public NotEnoughMoney(string notification) : base(notification)
        {

        }
    }
}
