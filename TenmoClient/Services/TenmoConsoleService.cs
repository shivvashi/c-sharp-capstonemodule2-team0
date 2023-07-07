using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TenmoClient.Models;


namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        
        public TenmoApiService apiService = new TenmoApiService("https://localhost:44315/");
        public ConsoleService console = new ConsoleService();



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
        

        public IList<int> DisplayPendingTransfers(IList<Transfer> transfers)
        {
            IList<Transfer> pendingList = new List<Transfer>();
            IList<int> listOfTransferIds = new List<int>();
            Console.WriteLine("-------------------------------------------\r\nPending Transfers\r\nID                               Amount\r\n-------------------------------------------");
            int myId = apiService.UserId;
            foreach(Transfer item in transfers)
            {
                if(item.TransferStatusDesc == "Pending")
                {
                    if (apiService.GetUsersByAccountId(item.AccountFrom)[0].UserId == myId)
                    {
                        Console.WriteLine($"{item.TransferId}          To: {apiService.GetUsersByAccountId(item.AccountTo)[0].Username}                $ {item.Amount}");
                    }
                    else
                    {
                        Console.WriteLine($"{item.TransferId}          From: {apiService.GetUsersByAccountId(item.AccountFrom)[0].Username}                $ {item.Amount}");
                    }
                    //Might need new transfer pending list
                    pendingList.Add(item);
                    listOfTransferIds.Add(item.TransferId);
                }
                
            }

            return listOfTransferIds;
        }

        public void DisplayTransfer(Transfer transfer)
        {
            Console.WriteLine("--------------------------------------------\r\nTransfer Details\r\n--------------------------------------------");
            Console.WriteLine($"Id: {transfer.TransferId}");
            Console.WriteLine($"From: {apiService.GetUsersByAccountId(transfer.AccountFrom)[0].Username}");
            Console.WriteLine($"To: {apiService.GetUsersByAccountId(transfer.AccountTo)[0].Username}");
            Console.WriteLine($"Type: {transfer.TransferTypeDesc}");
            Console.WriteLine($"Status: {transfer.TransferStatusDesc}");
            Console.WriteLine($"Amount: ${transfer.Amount}");
            Console.WriteLine();
            console.Pause();
        }

        

        

        public int ApproveOrReject(Transfer transfer)
        {
            Console.WriteLine($"\nTransfer Id: {transfer.TransferId}\nTo: {apiService.GetUsersByAccountId(transfer.AccountTo)[0].Username}     From: {apiService.GetUsersByAccountId(transfer.AccountFrom)[0].Username}\nAmount: ${transfer.Amount}");
            Console.WriteLine($"------------------------------------------- \n");
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't approve or reject");

            int statusYouWant = console.PromptForInteger("Please Choose an option");
            return statusYouWant;
        }

    }
}
