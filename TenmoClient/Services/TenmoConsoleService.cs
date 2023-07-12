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
            Console.Write($"3: View your pending requests (You have ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{apiService.GetPendingRequests(apiService.UserId).Count} ");
            Console.ResetColor();
            Console.Write("pending request(s)!)\n");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("-----------------------------");
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
            Console.Write($"$ {balance.ToString("0.00")}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void DisplayUsers(IList<User> users)
        {
            int index = 0;
            for(int i = 0; i < users.Count; i++)
            {
                if (users[i].UserId == apiService.UserId)
                {
                    index = i;
                }
            }
            
            users.RemoveAt(index);
            Console.WriteLine("|-------------- Users --------------|\r\n|    Id |    Username                  |\r\n|-----------------------------------|");
            foreach(User item in users)
            {
                Console.WriteLine($"  {item.UserId} |    {item.Username}");
            }
            Console.WriteLine("|-----------------------------------|");
        }
        

        public IList<int> DisplayPendingTransfers(IList<Transfer> transfers)
        {
            Console.Clear();
            IList<Transfer> pendingList = new List<Transfer>();
            IList<int> listOfTransferIds = new List<int>();
            Console.WriteLine("-------------------------------------------------\r\nPending Transfers\r\nID\t\tFrom/To\t\t\tAmount\r\n-------------------------------------------------");
            int myId = apiService.UserId;
            foreach(Transfer item in transfers)
            {
                if(item.TransferStatusDesc == "Pending")
                {
                    if (apiService.GetUsersByAccountId(item.AccountFrom)[0].UserId == myId)
                    {
                        Console.Write($"\n{item.TransferId}\t\t");
                        Console.Write($"To:   {apiService.GetUsersByAccountId(item.AccountTo)[0].Username}\t\t");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"- $ {item.Amount.ToString("0.00")}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"\n{item.TransferId}\t\t");
                        Console.Write($"From: {apiService.GetUsersByAccountId(item.AccountFrom)[0].Username}\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"+ $ {item.Amount.ToString("0.00")}");
                        Console.ResetColor();
                    }
                    //Might need new transfer pending list
                    pendingList.Add(item);
                    listOfTransferIds.Add(item.TransferId);
                }
                
            }
            Console.WriteLine("\n-------------------------------------------------");
            return listOfTransferIds;
        }

        public void DisplayTransfer(Transfer transfer)
        {
            Console.WriteLine("--------------------------------------------\r\nTransfer Details\r\n--------------------------------------------");
            Console.WriteLine($"Id:\t{transfer.TransferId}");
            Console.WriteLine($"From:\t{apiService.GetUsersByAccountId(transfer.AccountFrom)[0].Username}");
            Console.WriteLine($"To:\t{apiService.GetUsersByAccountId(transfer.AccountTo)[0].Username}");
            Console.WriteLine($"Type:\t{transfer.TransferTypeDesc}");
            Console.WriteLine($"Status:\t{transfer.TransferStatusDesc}");
            Console.WriteLine($"Amount:\t${transfer.Amount.ToString("0.00")}");
            Console.WriteLine();
            console.Pause();
        }

        
        public int ApproveOrReject(Transfer transfer)
        {
            Console.WriteLine($"\nTransfer Id: {transfer.TransferId}\nTo: {apiService.GetUsersByAccountId(transfer.AccountTo)[0].Username}     From: {apiService.GetUsersByAccountId(transfer.AccountFrom)[0].Username}\nAmount: ${transfer.Amount.ToString("0.00")}");
            Console.WriteLine($"------------------------------------------- \n");
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't approve or reject");
            Console.WriteLine($"------------------------------------------- \n");

            int statusYouWant = console.PromptForInteger("Please Choose an option",0,2);
            return statusYouWant;
        }

        public IList<int> DisplayPastTransfers(IList<Transfer> transfers)
        {
            List<int> pastTransferIds = new List<int>();
            Console.Clear();
            Console.WriteLine("-------------------------------------------------\r\nTransfers\r\nID\t\tFrom/To\t\t\tAmount\r\n-------------------------------------------------");
            foreach (Transfer item in transfers)
            {
                if (item.TransferStatusId == 2)
                {
                    if (apiService.GetUsersByAccountId(item.AccountFrom)[0].UserId == apiService.UserId)
                    {
                        Console.Write($"\n{item.TransferId}\t\t");

                        Console.Write($"TO:     {apiService.GetUsersByAccountId(item.AccountTo)[0].Username}\t\t");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"- $ {item.Amount.ToString("0.00")}");
                        Console.ResetColor();

                    }
                    else
                    {
                        Console.Write($"\n{item.TransferId}\t\t");

                        Console.Write($"FROM:   {apiService.GetUsersByAccountId(item.AccountFrom)[0].Username}\t\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"+ $ {item.Amount.ToString("0.00")}");
                        Console.ResetColor();
                    }
                    
                    pastTransferIds.Add(item.TransferId);
                }
            }
            Console.WriteLine("\n-------------------------------------------------");
            return pastTransferIds;
        }

        public int SelectTransfer(IList<int> pastTransferIds)
        {
            int userResponse = console.PromptForInteger("---------\r\nPlease enter transfer ID to view details (0 to cancel)");
            while (!pastTransferIds.Contains(userResponse))
            {
                if (userResponse == 0)
                {

                    return userResponse;
                }
                else
                {
                    userResponse = console.PromptForInteger("Please enter  transfer ID to view details (0 to cancel)");
                }
            }

            return userResponse;
        }

        


    }
}
