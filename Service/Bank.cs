using BancSystem.Currencies;
using BancSystem.Exceptions;
using BancSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static BancSystem.Service.Bank;

namespace BancSystem.Service
{
    public class Bank
    {
        public List<Client> clients = new List<Client>();
        public List<Employee> employees = new List<Employee>();
        public Dictionary<Client, List<Accaunt>> dataBaseClients = new Dictionary<Client, List<Accaunt>>();
        public Func<int, CurrencyType, CurrencyType, double> exchangeFunc;

        public void Add<T>(T person) where T : IPerson
        {
            try
            {
                var lokalClient = person as Client;
                var lokalEmploy = person as Employee;

                if (lokalClient != null)
                {
                    if (lokalClient.Age < 18)
                    {
                        throw new AgeLimitException("Сожалеем, но вы не можете стать клиентом нашего банка т.к. вам меньше 18 лет.");
                    }

                    clients.Add(lokalClient);
                }
                else if (lokalEmploy != null)
                {
                    if (lokalEmploy.Age < 18)
                    {
                        throw new AgeLimitException("Сожалеем, но вы не можете стать сотрудником нашего банка т.к. вам меньше 18 лет.");
                    }

                    employees.Add(lokalEmploy);
                }
            }
            catch(AgeLimitException e)
            {
                Console.WriteLine(e);
            }
        }

        public void MoneyTransfer(int sum, Accaunt firstAccaunt, Accaunt secondAccaunt, Func<int, CurrencyType, CurrencyType, double> exchangeFunc)
        {
            if (exchangeFunc == null)
            {
                Console.WriteLine("Делегат пустой");
            }
            else
            {
                try
                {
                    if (sum > firstAccaunt.CountMoney)
                    {
                        throw new NotEnoughMoney("Не достаточно денег на счету для данной операции");
                    }

                    if (firstAccaunt.CurrentCurrency.NameCurrency == secondAccaunt.CurrentCurrency.NameCurrency)
                    {
                        firstAccaunt.CountMoney -= sum;
                        secondAccaunt.CountMoney += sum;
                    }
                    else
                    {
                        firstAccaunt.CountMoney -= sum;
                        double exchangedSum = exchangeFunc(sum, firstAccaunt.CurrentCurrency, secondAccaunt.CurrentCurrency);
                        secondAccaunt.CountMoney += (int)exchangedSum;
                    }
                }
                catch(NotEnoughMoney e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void AddClintAccaunt(Client newClient, Accaunt newAccaunt)
        {
            if (newClient != null && newAccaunt != null)
            {
                try
                {
                    if (newClient.Age < 18)
                    {
                        throw new AgeLimitException("Вы не можете стать нашим клиентом т.к. вам меньше 18 лет");
                    }

                    if (dataBaseClients.ContainsKey(newClient))
                    {
                        dataBaseClients[newClient].Add(newAccaunt);
                    }
                    else
                    {
                        List<Accaunt> localListAccaunt = new List<Accaunt>() { newAccaunt };
                        dataBaseClients.Add(newClient, localListAccaunt);
                    }
                }
                catch (AgeLimitException e)
                {
                    Console.WriteLine(e);
                }
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
