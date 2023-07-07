using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                Environment.Exit(0);
                return false;
            }

            if (menuSelection == 1)
            {
                // View your current balance
                decimal balance = tenmoApiService.GetAccount(tenmoApiService.UserId).Balance;
                console.DisplayBalance(balance);
                Console.WriteLine();
                console.Pause();                               
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                List<Transfer> transfers = tenmoApiService.GetTransfers(tenmoApiService.UserId);
                List<int> pastTransferIds = new List<int>();

                Console.WriteLine("-------------------------------------------\r\nTransfers\r\nID          From/To                 Amount\r\n-------------------------------------------");
                foreach (Transfer item in transfers)
                {
                    if (item.TransferStatusId == 2)
                    {
                        if (tenmoApiService.GetUsersByAccountId(item.AccountFrom)[0].UserId == tenmoApiService.UserId)
                        {
                            Console.WriteLine($"{item.TransferId}          TO: {tenmoApiService.GetUsersByAccountId(item.AccountTo)[0].Username}          $ {item.Amount}");
                        }
                        else
                        {
                            Console.WriteLine($"{item.TransferId}          FROM: {tenmoApiService.GetUsersByAccountId(item.AccountFrom)[0].Username}          $ {item.Amount}");
                        }
                        pastTransferIds.Add(item.TransferId);
                    }
                }

                int userResponse = console.PromptForInteger("---------\r\nPlease enter transfer ID to view details (0 to cancel)");
                
                while (!pastTransferIds.Contains(userResponse))
                {
                    if (userResponse == 0)
                    {

                        return true;
                    }
                    else
                    {
                        userResponse = console.PromptForInteger("Please enter  transfer ID to view details (0 to cancel)");
                    }
                }

                Transfer transfer = tenmoApiService.GetTransferById(userResponse);
                console.DisplayTransfer(transfer);

            }

            if (menuSelection == 3)
            {
                Account myAccount = tenmoApiService.GetAccount(tenmoApiService.UserId);

                // View your pending requests
                IList<Transfer> transfers = tenmoApiService.GetTransfers(tenmoApiService.UserId);
                IList<int> transferIds =  console.DisplayPendingTransfers(transfers);
                int transferSelection = console.PromptForInteger("Please enter a transfer id to approve/reject (0 to cancel)");
                while (!transferIds.Contains(transferSelection))
                {
                    if(transferSelection == 0)
                    {
                        return true;
                    }
                    else
                    {
                        transferSelection = console.PromptForInteger("Please enter a transfer id to approve/reject (0 to cancel)");
                    }
                }

                Transfer selectedTransfer = tenmoApiService.GetTransferById(transferSelection);



                int statusYouWant = console.ApproveOrReject(selectedTransfer);



                while (statusYouWant == 1)
                {
                    if (selectedTransfer.Amount <= myAccount.Balance)
                    {
                        if (myAccount.AccountId == selectedTransfer.AccountFrom)
                        {
                            //Update transfer if approved
                            selectedTransfer.TransferStatusId = 2;
                            tenmoApiService.UpdateTransfer(selectedTransfer, selectedTransfer.TransferId);

                            //Get the Accounts to update
                            Account to = tenmoApiService.GetAccount(tenmoApiService.GetUsersByAccountId(selectedTransfer.AccountTo)[0].UserId);
                            Account from = tenmoApiService.GetAccount(tenmoApiService.GetUsersByAccountId(selectedTransfer.AccountFrom)[0].UserId);

                            //Updating the AccountTo
                            to.Balance += selectedTransfer.Amount;
                            tenmoApiService.UpdateAccountBalance(to, to.AccountId);

                            //Updating the AccountFrom
                            from.Balance -= selectedTransfer.Amount;
                            tenmoApiService.UpdateAccountBalance(from, from.AccountId);
                            console.PrintSuccess("Request was successfully approved");
                        }
                        else
                        {
                            console.PrintError("Can not approve transfer to yourself");
                            statusYouWant = console.ApproveOrReject(selectedTransfer);

                        }
                    }
                    else
                    {
                        console.PrintError("Balance too low to complete request. Please add more funds.");
                        statusYouWant = console.ApproveOrReject(selectedTransfer);
                    }
                }
                while (statusYouWant == 2)
                {
                    //Update transfer when rejected
                    selectedTransfer.TransferStatusId = 3;
                    tenmoApiService.UpdateTransfer(selectedTransfer, selectedTransfer.TransferId);
                    console.PrintSuccess("Request was successfully rejected");
                    console.Pause("\nPress any key to continue");
                    return true;

                }
                while (statusYouWant == 0)
                {
                    return true;
                }

                //Wait for user input, then return to Main Menu
                console.Pause("Press any key to go to Main Menu:");
            }
            if (menuSelection == 4)
            {
                // Send TE bucks
                Transfer newTransfer = new Transfer();
                newTransfer.AccountFrom = tenmoApiService.GetAccount(tenmoApiService.UserId).AccountId;
                newTransfer.TransferTypeId = 2;
                newTransfer.TransferStatusId = 2;

                IList<User> users = tenmoApiService.GetUsers();
                List<int> userIds = new List<int>();
                foreach(User item in users)
                {
                    userIds.Add(item.UserId);
                }
                userIds.Remove(tenmoApiService.UserId);
                console.DisplayUsers(users);

               
                int userResponse = console.PromptForInteger("Id of the user you are sending to (0 to cancel)");
                while (!userIds.Contains(userResponse))
                {
                    if(userResponse == 0)
                    {
                        return true;
                    }
                    else
                    {
                        console.PrintError("Not a valid userId. Please enter another number");
                        userResponse = console.PromptForInteger("Id of the user you are sending to (0 to cancel)");
                    }
                }
                
                int accountTo = tenmoApiService.GetAccount(userResponse).AccountId;
                newTransfer.AccountTo = accountTo;

                decimal amount = console.PromptForDecimal("Enter amount to send (press 0 to cancel)");
                while(amount >= tenmoApiService.GetAccount(tenmoApiService.UserId).Balance || amount <= .01M)
                {
                    if(amount == 0)
                    {
                        return true;
                    }
                    else
                    {
                        console.PrintError("Invalid money amount entered. Please try again.");
                        amount = console.PromptForDecimal("Enter amount to send");
                    }
                   
                }
                newTransfer.Amount = amount;

                tenmoApiService.CreateTransfer(newTransfer);

                //Update User's Balance               
                decimal decreaseAmount = -newTransfer.Amount;
                Account myAccount = tenmoApiService.GetAccount(tenmoApiService.UserId);
                myAccount.Balance += decreaseAmount;
                tenmoApiService.UpdateAccountBalance(myAccount, myAccount.AccountId);

                //Update receivers' balance
                decimal increaseAmount = newTransfer.Amount;
                Account receiver = tenmoApiService.GetAccount(userResponse);
                receiver.Balance += increaseAmount;
                tenmoApiService.UpdateAccountBalance(receiver, receiver.AccountId);

                console.PrintSuccess($"You sent ${increaseAmount} to {tenmoApiService.GetUserByUserId(userResponse).Username}");
                console.Pause("Press any key to continue: ");
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
                Transfer newRequest = new Transfer();
                newRequest.TransferTypeId = 1;
                newRequest.TransferStatusId = 1;
                newRequest.AccountTo = tenmoApiService.GetAccount(tenmoApiService.UserId).AccountId;
                //Get Users and Display Them
                IList<User> list = tenmoApiService.GetUsers();
                List<int> userIds = new List<int>();
                foreach (User item in list)
                {
                    userIds.Add(item.UserId);
                }
                userIds.Remove(tenmoApiService.UserId);
                console.DisplayUsers(list);
                
                int userResponse = console.PromptForInteger("Id of the user you are requesting from (press 0 to cancel)");
                while (!userIds.Contains(userResponse))
                {
                    if (userResponse == 0)
                    {
                        return true;
                    }
                    else
                    {
                        console.PrintError("Not a valid userId. Please enter another number");
                        userResponse = console.PromptForInteger("Id of the user you are sending to (press 0 to cancel)");
                    }
                }

                //Get AccountId from user input and set to the AccountFrom 
                int requestFrom = tenmoApiService.GetAccount(userResponse).AccountId;
                newRequest.AccountFrom = requestFrom;

                //Prompt for amount and set the Property
                decimal amount = console.PromptForDecimal("Enter amount to request (press 0 to cancel)");
                while (amount <= 0)
                {
                    if(amount == 0)
                    {
                        return true;
                    }
                    else
                    {
                        console.PrintError("Invalid money amount entered. Can not request negative amount. Please try again.");
                        amount = console.PromptForDecimal("Enter amount to request (press 0 to cancel)");
                    }
                }
                newRequest.Amount = amount;
                tenmoApiService.CreateTransfer(newRequest);
                console.PrintSuccess("Your request was successfully submitted.");
                console.Pause("Press any key to continue: ");
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();                
                console.PrintSuccess("You are now logged out");
                
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }
    }
}
