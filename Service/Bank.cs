using BancSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Service
{
    public class Bank
    {
        public List<Client> clients = new List<Client>();
        public List<Employee> employees = new List<Employee>();

        public void Add<T>(T person) where T : IPerson
        {
            var lokalClient = person as Client;
            var lokalEmploy = person as Employee;

            if (lokalClient != null)
            {
                clients.Add(lokalClient);
            }
            else if (lokalEmploy != null)
            {
                employees.Add(lokalEmploy);
            }
        }

        public Client FindClient(IPerson person)
        {
            return (Client)FindPerson<IPerson>(person);
        }

        public Employee FindEmploy(IPerson person)
        {
            return (Employee)FindPerson<IPerson>(person);
        }

        private IPerson FindPerson<T>(T person) where T : IPerson
        {
            var lokalClient = person as Client;
            var lokalEmploy = person as Employee;

            if (lokalClient != null)
            {
                int indexFindClient = clients.IndexOf(lokalClient);

                return clients[indexFindClient];
            }
            else if (lokalEmploy != null)
            { 
                int indexFindEmployee = employees.IndexOf(lokalEmploy);

                return employees[indexFindEmployee];
            }
            else
            {
                return null;
            }
        }
    }
}
