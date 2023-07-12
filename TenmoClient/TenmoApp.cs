using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection.Metadata;
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
                DisplayCurrentBalance();                          
            }

            if (menuSelection == 2)
            {
                // View your past transfers
                DisplayPastTransfers();
            }

            if (menuSelection == 3)
            {
                //View and Approve/Reject Requests
                ViewPendingRequests();
            }
            if (menuSelection == 4)
            {
                // Send TE bucks
                SendMoney();                
            }

            if (menuSelection == 5)
            {
                // Request TE bucks
                RequestMoney();
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();                
                console.PrintSuccess("You are now logged out");                
            }

            return true;    // Keep the main menu loop going
        }


        private void SendMoney()
        {
            Transfer newTransfer = new Transfer();
            newTransfer.AccountFrom = tenmoApiService.GetAccount(tenmoApiService.UserId).AccountId;
            newTransfer.TransferTypeId = 2;
            newTransfer.TransferStatusId = 2;

            //Get list of users and their Ids and Display them.
            IList<User> users = tenmoApiService.GetUsers();
            IList<int> userIds = GetListOfUserIds(users);
            console.DisplayUsers(users);

            int userResponse = PromptForValidUserId(userIds);
            if (userResponse == 0)
            {
                return;
            }
            int accountTo = tenmoApiService.GetAccount(userResponse).AccountId;
            newTransfer.AccountTo = accountTo;


            decimal amount = PromptForValidSendAmount();
            if(amount == 0)
            {
                return;
            }
            newTransfer.Amount = amount;

            tenmoApiService.CreateTransfer(newTransfer);

            UpdateBalances(newTransfer,userResponse);

            console.PrintSuccess($"You sent ${newTransfer.Amount.ToString("0.00")} to {tenmoApiService.GetUserByUserId(userResponse).Username}");
            console.Pause("Press any key to continue: ");
        }

        private void RequestMoney()
        {
            Transfer newRequest = new Transfer();
            newRequest.TransferTypeId = 1;
            newRequest.TransferStatusId = 1;
            newRequest.AccountTo = tenmoApiService.GetAccount(tenmoApiService.UserId).AccountId;

            IList<User> users = tenmoApiService.GetUsers();
            IList<int> userIds = GetListOfUserIds(users);
            console.DisplayUsers(users);

            int userResponse = PromptForValidUserId(userIds);
            if (userResponse == 0)
            {
                return;
            }
            int accountFrom = tenmoApiService.GetAccount(userResponse).AccountId;
            newRequest.AccountFrom = accountFrom;

            decimal amount = PromptForValidRequestAmount();

            newRequest.Amount = amount;
            tenmoApiService.CreateTransfer(newRequest);
            console.PrintSuccess("Your request was successfully submitted.");
            console.Pause("Press any key to continue: ");
        }

        private void ViewPendingRequests()
        {
            Account myAccount = tenmoApiService.GetAccount(tenmoApiService.UserId);

            // View your pending requests
            IList<Transfer> transfers = tenmoApiService.GetTransfers(tenmoApiService.UserId);
            IList<int> transferIds = console.DisplayPendingTransfers(transfers);

            int transferSelection = PromptForValidTransferId(transferIds);
            if(transferSelection == 0)
            {
                return;
            }

            Transfer selectedTransfer = tenmoApiService.GetTransferById(transferSelection);

            int statusYouWant = console.ApproveOrReject(selectedTransfer);

            UpdateTransfer(selectedTransfer,statusYouWant);
        }


        private IList<int> GetListOfUserIds(IList<User> list)
        {
            List<int> userIds = new List<int>();
            foreach (User item in list)
            {
                userIds.Add(item.UserId);
            }

            return userIds;
        }



        private int PromptForValidUserId(IList<int> userIds)
        {
            int userResponse = console.PromptForInteger("Id of the user you are sending to (0 to cancel)");
            while (!userIds.Contains(userResponse))
            {
                if (userResponse == 0)
                {
                    return userResponse;
                }
                else
                {
                    console.PrintError("Not a valid userId. Please enter another number");
                    userResponse = console.PromptForInteger("Id of the user you are sending to (0 to cancel)");
                }
            }

            return userResponse;
        }

        private int PromptForValidTransferId(IList<int> transferIds)
        {
            int transferSelection = console.PromptForInteger("Please enter a transfer id to approve/reject (0 to cancel)");
            while (!transferIds.Contains(transferSelection))
            {
                if (transferSelection == 0)
                {
                    return transferSelection;
                }
                else
                {
                    transferSelection = console.PromptForInteger("Please enter a transfer id to approve/reject (0 to cancel)");
                }
            }
            return transferSelection;
        }



        private decimal PromptForValidSendAmount()
        {
            decimal amount = console.PromptForDecimal("Enter amount to send (press 0 to cancel)");
            while (amount >= tenmoApiService.GetAccount(tenmoApiService.UserId).Balance || amount <= .01M)
            {
                if (amount == 0)
                {
                    return amount;
                }
                else
                {
                    console.PrintError("Invalid money amount entered. Please try again.");
                    amount = console.PromptForDecimal("Enter amount to send");
                }

            }
            return amount;
        }



        private decimal PromptForValidRequestAmount()
        {
            decimal amount = console.PromptForDecimal("Enter amount to send (press 0 to cancel)");
            while (amount <= 0)
            {
                if (amount == 0)
                {
                    return amount;
                }
                else
                {
                    console.PrintError("Invalid money amount entered. Please try again.");
                    amount = console.PromptForDecimal("Enter amount to send");
                }

            }
            return amount;
        }


        private void DisplayPastTransfers()
        {
            IList<Transfer> transfers = tenmoApiService.GetTransfers(tenmoApiService.UserId);
            IList<int> pastTransferIds = console.DisplayPastTransfers(transfers);
            int userResponse = console.SelectTransfer(pastTransferIds);
            if (userResponse == 0)
            {
                return;
            }
            Transfer transfer = tenmoApiService.GetTransferById(userResponse);
            console.DisplayTransfer(transfer);
        }

        private void DisplayCurrentBalance()
        {
            decimal balance = tenmoApiService.GetAccount(tenmoApiService.UserId).Balance;
            console.DisplayBalance(balance);
            Console.WriteLine();
            console.Pause("Press any button to continue");
        }

        private void UpdateBalances(Transfer newTransfer, int userResponse)
        {
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
        }

        private void UpdateTransfer(Transfer selectedTransfer, int statusYouWant)
        {
            Account myAccount = tenmoApiService.GetAccount(tenmoApiService.UserId);
            //Check if they are allowed to update transfer to selected status
            while (!isAcceptableAction(selectedTransfer,statusYouWant))
            {
               statusYouWant = console.ApproveOrReject(selectedTransfer);
            }
            if(statusYouWant == 1)
            {
                //They are able to approve. So update transfer.
                selectedTransfer.TransferStatusId = 2;
                tenmoApiService.UpdateTransfer(selectedTransfer, selectedTransfer.TransferId);

                //Once approved update account balances
                UpdateBalances(selectedTransfer, tenmoApiService.GetUsersByAccountId(selectedTransfer.AccountTo)[0].UserId);
                console.PrintSuccess($"\nTransfer Approved. Your current balance is: ${myAccount.Balance}\n");
                console.Pause("Press any key to continue");
            }
            else if(statusYouWant == 2)
            {
                //Update account to status rejected
                selectedTransfer.TransferStatusId = 3;
                tenmoApiService.UpdateTransfer(selectedTransfer, selectedTransfer.TransferId);
                console.PrintSuccess("\nTransfer Rejected\n");
                console.Pause("Press any key to continue");
            }
            else if(statusYouWant == 0)
            {
                //Go back to Main Menu
                return;
            }
            
        }

        private bool isAcceptableAction(Transfer selectedTransfer, int statusYouWant)
        {
            if (selectedTransfer.AccountTo == tenmoApiService.GetAccount(tenmoApiService.UserId).AccountId && statusYouWant == 1)
            {
                console.PrintError("Can not approve transfer to yourself.");
                return false;
            }
            else if (selectedTransfer.Amount > tenmoApiService.GetAccount(tenmoApiService.UserId).Balance && statusYouWant == 1)
            {
                console.PrintError("Insufficient Funds to complete transfer.");
                return false;
            }
            else
            {
                return true;
            }
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
