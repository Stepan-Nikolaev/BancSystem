using BancSystem.Currencies;
using Newtonsoft.Json;
using BancSystem.Generators;
using BancSystem.Models;
using BancSystem.Service;
using System.Collections.Generic;
using System;

namespace BancSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Bank bank = new Bank();
            List<Client> clients = new List<Client>();
            List<Employee> employees = new List<Employee>();
            Dictionary<int, List<Accaunt>> dataBaseClients = new Dictionary<int, List<Accaunt>>();
            Generator generator = new Generator();

            dataBaseClients = generator.GetGeneratedDictionary(100);
            clients = generator.GetGeneratedListClients(100);
            employees = generator.GetGeneratedListEmployee(100);

            foreach (var client in clients)
            {
                bank.Add(client);
            }

            foreach (var employee in employees)
            {
                bank.Add(employee);
            }

            if (bank.GetClientsDictionaryFromFile() == null)
            {
                bank.TransferDictionaryInFile(dataBaseClients);
            }

            Dictionary<int, List<Accaunt>> newDataBaseClients = bank.GetClientsDictionaryFromFile();
        }
    }
}
