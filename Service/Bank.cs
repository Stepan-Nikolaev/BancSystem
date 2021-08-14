using BancSystem.Currencies;
using BancSystem.Exceptions;
using BancSystem.Models;
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
        public Dictionary<Client, List<Accaunt>> DataBaseClients = new Dictionary<Client, List<Accaunt>>();
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

            try
            {
                using (FileStream fileStream = new FileStream(DataClients, FileMode.Open))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string readTextDataClients = System.Text.Encoding.Default.GetString(array);
                    string[] newClient = readTextDataClients.Split("@", StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < (newClient.Length); i++)
                    {
                        Client lockalClient = new Client() { Name = newClient[i++], Surname = newClient[i++], Patronymic = newClient[i++], Age = Convert.ToInt32(newClient[i++]), PassportID = Convert.ToInt32(newClient[i]) };
                        Clients.Add(lockalClient);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Вы будете первым клиентом.");
            }

            try
            {
                using (FileStream fileStream = new FileStream(DataEmployees, FileMode.Open))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string readTextDataEmployees = System.Text.Encoding.Default.GetString(array);
                    string[] newEmploy = readTextDataEmployees.Split("@");

                    for (int i = 0; i < (newEmploy.Length); i++)
                    {
                        Employee lockalEmploy = new Employee() { Name = newEmploy[i++], Surname = newEmploy[i++], Patronymic = newEmploy[i++], Age = Convert.ToInt32(newEmploy[i++]), PassportID = Convert.ToInt32(newEmploy[i++]), Position = newEmploy[i++], WorkExperience = Convert.ToInt32(newEmploy[i]) };
                        Employees.Add(lockalEmploy);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Вы будете первым сотрудником.");
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
                        string textDataLokalClient = ($"@{lokalClient.Name}@{lokalClient.Surname}@{lokalClient.Patronymic}@{lokalClient.Age}@{lokalClient.PassportID}");

                        using (FileStream fileStream = new FileStream(DataClients, FileMode.Append))
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
                        string textDataLokalEmploy = ($"@{lokalEmploy.Name}@{lokalEmploy.Surname}@{lokalEmploy.Patronymic}@{lokalEmploy.Age}@{lokalEmploy.PassportID}@{lokalEmploy.Position}@{lokalEmploy.WorkExperience}");

                        using (FileStream fileStream = new FileStream(DataEmployees, FileMode.Append))
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

                    if (DataBaseClients.ContainsKey(newClient))
                    {
                        DataBaseClients[newClient].Add(newAccaunt);
                    }
                    else
                    {
                        List<Accaunt> localListAccaunt = new List<Accaunt>() { newAccaunt };
                        DataBaseClients.Add(newClient, localListAccaunt);
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

        public void TransferDictionaryInFile(Dictionary<Client, List<Accaunt>> dataBaseClients)
        {
            foreach (var clientAccaunts in dataBaseClients)
            {
                string dataLokalClientText = ($"*@{clientAccaunts.Key.Name}@{clientAccaunts.Key.Surname}@{clientAccaunts.Key.Patronymic}@{clientAccaunts.Key.Age}@{clientAccaunts.Key.PassportID}");
                string accauntsText = "";

                foreach (var accaunt in clientAccaunts.Value)
                {
                    accauntsText += $"*@{accaunt.CountMoney}@{accaunt.CurrentCurrency.NameCurrency}";
                }

                string clientAccauntsText = $"!{dataLokalClientText}{accauntsText}";

                using (FileStream fileStream = new FileStream(ClientsDictionary, FileMode.Append))
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(clientAccauntsText);
                    fileStream.Write(array, 0, array.Length);
                }
            }
        }

        public Dictionary<Client, List<Accaunt>> GetClientsDictionaryFromFile()
        {
            try
            {
                Dictionary<Client, List<Accaunt>> lockalDataBaseClients = new Dictionary<Client, List<Accaunt>>();

                using (FileStream fileStream = new FileStream(ClientsDictionary, FileMode.Open))
                {
                    byte[] array = new byte[fileStream.Length];
                    fileStream.Read(array, 0, array.Length);
                    string readClientsDictionary = System.Text.Encoding.Default.GetString(array);
                    string[] newClientAccauntsArray = readClientsDictionary.Split("!", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var clientAccaunts in newClientAccauntsArray)
                    {
                        string[] clientOrAccaunt = clientAccaunts.Split("*", StringSplitOptions.RemoveEmptyEntries);

                        Client lockalClient = new Client();
                        List<Accaunt> lockalAccauntsList = new List<Accaunt>();

                        for (int i = 0; i < clientOrAccaunt.Length; i++)
                        {

                            if (i == 0)
                            {
                                string[] client = clientOrAccaunt[i].Split("@", StringSplitOptions.RemoveEmptyEntries);

                                lockalClient = new Client() { Name = client[0], Surname = client[1], Patronymic = client[2], Age = Convert.ToInt32(client[3]), PassportID = Convert.ToInt32(client[4]) };

                            }
                            else
                            {
                                string[] accaunt = clientOrAccaunt[i].Split("@", StringSplitOptions.RemoveEmptyEntries);

                                CurrencyType localCurrency = new Euro();

                                switch (accaunt[1])
                                {
                                    case "Евро":
                                        localCurrency = new Euro();
                                        break;
                                    case "Гривна":
                                        localCurrency = new Hryvnia();
                                        break;
                                    case "Рубль":
                                        localCurrency = new Ruble();
                                        break;
                                }

                                lockalAccauntsList.Add(new Accaunt() { CountMoney = Convert.ToInt32(accaunt[0]), CurrentCurrency = localCurrency });
                            }
                        }

                        lockalDataBaseClients.Add(lockalClient, lockalAccauntsList);
                    }
                }

                return lockalDataBaseClients;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файла библиотеки не существует.");
                return null;
            }
        }
    }
}
