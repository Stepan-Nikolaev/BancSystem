using BancSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Models
{
    public class Client : IPerson
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public int Age { get; set; }
        public int PassportID { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Client))
            {
                return false;
            }

            Client verifiedCient = (Client)obj;

            return PassportID == verifiedCient.PassportID;
        }

        public override int GetHashCode()
        {
            return PassportID;
        }

    }
}
