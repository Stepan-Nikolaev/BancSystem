using BancSystem.Currencies;
using BancSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Generators
{
    public class Generator
    {
        public Dictionary<Client, List<Accaunt>> GetGeneratedDictionary(int lengthDictionary)
        {
            Dictionary<Client, List<Accaunt>> dataBaseClients = new Dictionary<Client, List<Accaunt>>();

            for (int i = 0; i < lengthDictionary; i++)
            {
                Accaunt accaunt1 = new Accaunt() { CountMoney = i, CurrentCurrency = new Euro() };
                Accaunt accaunt2 = new Accaunt() { CountMoney = i, CurrentCurrency = new Hryvnia() };
                Accaunt accaunt3 = new Accaunt() { CountMoney = i, CurrentCurrency = new Ruble() };
                List<Accaunt> accaunts = new List<Accaunt> { accaunt1, accaunt2, accaunt3 };
                Client client = new Client() { Name = "Иван", Surname = "Иванов", Patronymic = "Иванович", Age = (i + 18), PassportID = i };
                dataBaseClients.Add(client, accaunts);
            }

            return dataBaseClients;
        }

        public List<Client> GetGeneratedListClients(int lengthList)
        {
            List<Client> clients = new List<Client>();

            for (int i = 0; i < lengthList; i++)
            {
                Client client = new Client() { Name = "Иван", Surname = "Иванов", Patronymic = "Иванович", Age = (i + 18), PassportID = i };
                clients.Add(client);
            }

            return clients;
        }

        public List<Employee> GetGeneratedListEmployee(int lengthList)
        {
            List<Employee> employees = new List<Employee>();

            for (int i = 0; i < lengthList; i++)
            {
                Employee employee = new Employee() { Name = "Иван", Surname = "Иванов", Patronymic = "Иванович", Age = (i + 18), PassportID = i, Position = "Должность", WorkExperience = i };
                employees.Add(employee);
            }

            return employees;
        }
    }
}
