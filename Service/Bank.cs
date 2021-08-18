using BancSystem.Currencies;
using BancSystem.Exceptions;
using BancSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BancSystem.Service.Bank;

namespace BancSystem.Service
{
    public class Bank
    {
        public static string PathFileBankSystem = Path.Combine("D:", "Степапка", "DEX", "DexPractice", "BancSystem", "BancSystem", "DataBaseBank");
        public static string DataClients = Path.Combine("D:", "Степапка", "DEX", "DexPractice", "BancSystem", "BancSystem", "DataBaseBank", "ClientsDataBase.txt");
        public static string DataEmployees = Path.Combine("D:", "Степапка", "DEX", "DexPractice", "BancSystem", "BancSystem", "DataBaseBank", "EmployeesDataBase.txt");
        public static string ClientsDictionary = Path.Combine("D:", "Степапка", "DEX", "DexPractice", "BancSystem", "BancSystem", "DataBaseBank", "ClientsDictionary.txt");
        public List<Client> Clients = new List<Client>();
        public List<Employee> Employees = new List<Employee>();
        public Dictionary<int, List<Accaunt>> DataBaseClients = new Dictionary<int, List<Accaunt>>();
        public Func<int, CurrencyType, CurrencyType, double> ExchangeFunc;
        public DirectoryInfo directoryInfo = new DirectoryInfo(PathFileBankSystem);
        public DirectoryInfo clientsDirectoryInfo = new DirectoryInfo(DataClients);
        public DirectoryInfo employeesDirectoryInfo = new DirectoryInfo(DataEmployees);

        public Bank()
        {
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            if (GetClientListFromFile() != null)
            {
                Clients = GetClientListFromFile();
            }

            if (GetEmployeeListFromFile() != null)
            {
                Employees = GetEmployeeListFromFile();
            }

            if (GetClientsDictionaryFromFile() != null)
            {
                DataBaseClients = GetClientsDictionaryFromFile();
            }
        }

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

                    if (Clients.Contains(lokalClient))
                    {
                        Console.WriteLine("Такой клиент уже существует");
                    }
                    else
                    {
                        Clients.Add(lokalClient);
                        string textDataLokalClient = JsonConvert.SerializeObject(Clients);

                        using (FileStream fileStream = new FileStream(DataClients, FileMode.Create))
                        {
                            byte[] array = System.Text.Encoding.Default.GetBytes(textDataLokalClient);
                            fileStream.Write(array, 0, array.Length);
                        }
                    }
                }
                else if (lokalEmploy != null)
                {
                    if (lokalEmploy.Age < 18)
                    {
                        throw new AgeLimitException("Сожалеем, но вы не можете стать сотрудником нашего банка т.к. вам меньше 18 лет.");
                    }

                    if (Employees.Contains(lokalEmploy))
                    {
                        Console.WriteLine("Такой сотрудник уже существует");
                    }
                    else
                    {
                        Employees.Add(lokalEmploy);
                        string textDataLokalEmploy = JsonConvert.SerializeObject(Employees);

                        using (FileStream fileStream = new FileStream(DataEmployees, FileMode.Create))
                        {
                            byte[] array = System.Text.Encoding.Default.GetBytes(textDataLokalEmploy);
                            fileStream.Write(array, 0, array.Length);
                        }
                    }
                }
            }
            catch (AgeLimitException e)
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
                catch (NotEnoughMoney e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void AddClientAccaunt(Client newClient, Accaunt newAccaunt)
        {
            if (newClient != null && newAccaunt != null)
            {
                try
                {
                    if (newClient.Age < 18)
                    {
                        throw new AgeLimitException("Вы не можете стать нашим клиентом т.к. вам меньше 18 лет");
                    }

                    if (DataBaseClients.ContainsKey(newClient.PassportID))
                    {
                        DataBaseClients[newClient.PassportID].Add(newAccaunt);
                    }
                    else
                    {
                        List<Accaunt> localListAccaunt = new List<Accaunt>() { newAccaunt };
                        DataBaseClients.Add(newClient.PassportID, localListAccaunt);
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
                int indexFindClient = Clients.IndexOf(lokalClient);

                return Clients[indexFindClient];
            }
            else if (lokalEmploy != null)
            {
                int indexFindEmployee = Employees.IndexOf(lokalEmploy);

                return Employees[indexFindEmployee];
            }
            else
            {
                return null;
            }
        }

        public void TransferDictionaryInFile(Dictionary<int, List<Accaunt>> dataBaseClients)
        {
            string dictionaryText = JsonConvert.SerializeObject(dataBaseClients);

            using (FileStream fileStream = new FileStream(ClientsDictionary, FileMode.Append))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(dictionaryText);
                fileStream.Write(array, 0, array.Length);
            }
        }

        public Dictionary<int, List<Accaunt>> GetClientsDictionaryFromFile()
        {
            try
            {
                Dictionary<int, List<Accaunt>> lockalDataBaseClients = new Dictionary<int, List<Accaunt>>();

                using (FileStream fileStream = new FileStream(ClientsDictionary, FileMode.Open))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string readClientsDictionary = System.Text.Encoding.Default.GetString(array);
                    lockalDataBaseClients = JsonConvert.DeserializeObject<Dictionary<int, List<Accaunt>>>(readClientsDictionary);
                }

                return lockalDataBaseClients;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файла библиотеки не существует.");
                return null;
            }
        }

        private List<Client> GetClientListFromFile()
        {
            try
            {
                List<Client> clients = new List<Client>();

                using (FileStream fileStream = new FileStream(DataClients, FileMode.Open))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string readTextDataClients = System.Text.Encoding.Default.GetString(array);
                    clients = JsonConvert.DeserializeObject<List<Client>>(readTextDataClients);
                }

                return clients;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Вы будете первым клиентом.");

                return null;
            }
        }

        private List<Employee> GetEmployeeListFromFile()
        {
            try
            {
                List<Employee> employees = new List<Employee>();

                using (FileStream fileStream = new FileStream(DataEmployees, FileMode.Open))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string readTextDataEmployees = System.Text.Encoding.Default.GetString(array);
                    employees = JsonConvert.DeserializeObject<List<Employee>>(readTextDataEmployees);
                }

                return employees;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Вы будете первым сотрудником.");

                return null;
            }
        }
    }
}
