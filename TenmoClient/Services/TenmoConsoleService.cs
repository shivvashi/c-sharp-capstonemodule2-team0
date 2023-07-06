using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        

        
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...
        public void DisplayBalance(decimal balance)
        {
            Console.Write($"Your current account balance is: ");
            ConsoleColor color = balance > 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = color;
            Console.Write($"$ {balance}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void DisplayUsers(IList<User> users)
        {
            Console.WriteLine("|-------------- Users --------------|\r\n|    Id | Username                  |\r\n|-------+---------------------------|");
            foreach(User item in users)
            {
                Console.WriteLine($"|  {item.UserId} | {item.Username}                   |");
            }
            Console.WriteLine("|-----------------------------------|");
        }
        

        //public void DisplaySingleTransfer(Transfer transfer)
        //{
        //    Console.WriteLine("--------------------------------------------\r\nTransfer Details\r\n--------------------------------------------");
        //    Console.WriteLine($"Id: {transfer.TransferId}");
        //    Console.WriteLine($"From: {api.GetUsersByAccountId(transfer.AccountFrom)[0].Username}");
        //    Console.WriteLine($"To: {api.GetUsersByAccountId(transfer.AccountTo)[0].Username}");
        //    Console.WriteLine($"Type: {transfer.TransferTypeDesc}");
        //    Console.WriteLine($"Status: {transfer.TransferStatusDesc}");
        //    Console.WriteLine($"Amount: ${transfer.Amount}");
           

        //}

    }
}
