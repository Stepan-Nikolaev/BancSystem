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
    }
}
