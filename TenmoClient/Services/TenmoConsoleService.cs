﻿using System;
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

        public void DisplayTransfers(List<Transfer> transfers)
        {
            
            Console.WriteLine("-------------------------------------------\r\nTransfers\r\nID          From/To                 Amount\r\n-------------------------------------------");
            foreach (Transfer item in transfers)
            {
                if (item.TransferTypeId == 1)
                {
                    Console.WriteLine($"{item.TransferId}          From: {item.AccountFrom}           $ {item.Amount}");
                }
                else
                {
                    Console.WriteLine($"{item.TransferId}          To: {item.AccountTo}          $ {item.Amount}");
                }
            }         
        }

        public void DisplaySingleTransfer(Transfer transfer)
        {
            

        }

    }
}
