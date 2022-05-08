using BankApp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace BankApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            List<User> users = File.ReadAllLines("osoby.csv").Skip(1).Select(x => CreateUser(x)).ToList();
            List<Account> accounts = File.ReadAllLines("konta.csv").Skip(1).Select(x => CreateAccount(x,users)).ToList();
            foreach (var user in users)
            {
                AssignAccountToUser(accounts, user);
            }

            #region 1. wypisywac liste kont uzytkownika
            Console.WriteLine("\n-------1. wypisywac liste kont uzytkownika---------\n");
            UserAccount(1, users);
            UserAccount(2, users);
            UserAccount(3, users);
            #endregion

            #region 2. umozliwiać dokonywanie wplat, wyplat z konta
            Console.WriteLine("\n-------2. umozliwiać dokonywanie wplat, wyplat z konta---------\n");
            Withdraw(1, 100, users, 12441231451);
            Payment(1, 200, users, 12441231451);
            #endregion

            #region 3. wypisać listę konta zablokowanych
            Console.WriteLine("\n-------3. wypisać listę konta zablokowanych---------\n");
            LockedAccounts(accounts);
            #endregion

            #region 4. wygegerowac raport o uzytkownikach wraz z saldem na poszczegolnych kontach
            Console.WriteLine("\n-------4. wygegerowac raport o uzytkownikach wraz z saldem na poszczegolnych kontach---------\n");
            UsersReport(users);

            #endregion

        }



        public static User CreateUser(string userCSV)
        {
            string[] values = userCSV.Split(';');
            User user = new User();
            user.UserId = int.Parse(values[0]);
            user.FirstName = values[1];
            user.LastName = values[2];
            user.Pesel = values[3];
            user.Adress = values[4];
            return user;
        }

        public static Account CreateAccount(string accountCSV, List<User> users)
        {
            string[] values = accountCSV.Split(';');
            Account account = new Account();
            User newUser = users.FirstOrDefault(x => x.UserId == int.Parse(values[1]));
            account.AccountId = long.Parse(values[0]);
            account.User = newUser;
            account.CurrencyType = EnumCurrencyType(values[2]);
            account.Balance = decimal.Parse(values[3]);
            account.AccountLock = AccoutLock(values[4]);
            return account;
        }

        public static Enums.CurrencyType EnumCurrencyType(string x)
        {
            if (x == "PLN")
                return Enums.CurrencyType.PLN;
            if (x == "EUR")
                return Enums.CurrencyType.EUR;
            if (x == "USD")
                return Enums.CurrencyType.USD;
            return Enums.CurrencyType.PLN;
        }
        public static bool AccoutLock(string x)
        {
            if (x == "NIE")
                return false;
            else 
                return true;
        }
        public static void AssignAccountToUser(List<Account> accounts, User user)
        {
            var listOfAccounts = accounts.Where(x=> x.User.UserId == user.UserId).ToList();
            user.Accounts = listOfAccounts;
        }
        public static void LockedAccounts(List<Account> list)
        {
            List<Account> lockedAccounts = list.Where(x => x.AccountLock == true).ToList();
            lockedAccounts.Select(x => $"Id zablokowanego konta: {x.AccountId}, posiadacz: {x.User.FirstName} {x.User.LastName}\n").ToList().ForEach(Console.WriteLine);
        }
        public static void UserAccount(int i, List<User> list)
        {
            User newUser = list.FirstOrDefault(x => x.UserId == i);
            List<Account> accountList = newUser.Accounts.ToList();
            
            Console.WriteLine(newUser.FirstName + " " + newUser.LastName + ", id = "+ newUser.UserId);

            accountList.Select(x => $"Numer konta: {x.AccountId}, Saldo: {x.Balance}, Waluta: {x.CurrencyType}\n").ToList().ForEach(Console.WriteLine);
        }

        public static void Withdraw(int id, decimal value, List<User> list, long AccountId)
        {
            User user = list.FirstOrDefault(x => x.UserId == id);
            List<Account> account = user.Accounts.ToList();

            account.Select(x => x.AccountId == AccountId && value <= x.Balance ? $"Po wypłacie {value} {x.CurrencyType}, na koncie {x.AccountId} pozostało: {x.Balance -= value} {x.CurrencyType}" : "").ToList().ForEach(Console.WriteLine);
        }

        public static void Payment(int id, decimal value, List<User> list, long AccountId)
        {
            User user = list.FirstOrDefault(x => x.UserId == id);
            List<Account> account = user.Accounts.ToList();

            account.Select(x => x.AccountId == AccountId ? $"Po wpłacie {value} {x.CurrencyType} na konto {x.AccountId}, posiadasz: {x.Balance += value} {x.CurrencyType}" : "").ToList().ForEach(Console.WriteLine);
        }

        public static void UsersReport(List<User> users)
        {
            foreach (var user in users)
            {
                List<Account> list = user.Accounts.ToList();
                list.Select(x => $"Id: {x.User.UserId}, Name: {x.User.FirstName} {x.User.LastName}, PESEL: {x.User.Pesel}, Adres: {x.User.Adress}\n Konto: {x.AccountId}, Saldo: {x.Balance}, Waluta: {x.CurrencyType}").ToList().ForEach(x => Console.WriteLine(x));
            }
        }
    }
}