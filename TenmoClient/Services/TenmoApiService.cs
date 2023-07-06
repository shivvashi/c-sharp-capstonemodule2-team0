using RestSharp;
using System.Collections.Generic;
using System.Reflection.Metadata;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl)
        {
            this.ApiUrl = apiUrl;
        }

        // Add methods to call api here...

        //working -- Get Account Info, for user
        public Account GetAccount(int id)
        {
            RestRequest request = new RestRequest($"account/{id}");
            IRestResponse<Account> response = client.Get<Account>(request);
            CheckForError(response);
            return response.Data;
        }

        //working -- Get all transfers for a specific user
        public List<Transfer> GetTransfers(int userId)
        {
            RestRequest request = new RestRequest($"/{userId}/transfers");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            CheckForError(response);
            return response.Data;
        }
        //working -- Get transfer by specific transfer id
        public Transfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest($"transfers/{transferId}");
            IRestResponse<Transfer> response = client.Get<Transfer>(request);
            CheckForError(response);
            return response.Data;
        }

        //public string GetFromUserByAccountId(int id)
        //{
        //    RestRequest request = new RestRequest($"transfers/from/{id}");
        //    IRestResponse<string> response = client.Get<string>(request);
        //    CheckForError(response);
        //    return response.Data;
        //}

        //public string GetToUserByAccountId(int id)
        //{
        //    RestRequest request = new RestRequest($"transfers/to/{id}");
        //    IRestResponse<string> response = client.Get<string>(request);
        //    CheckForError(response);
        //    return response.Data;
        //}

        public User GetUserByUserId(int id)
        {
            RestRequest request = new RestRequest($"user/{id}");
            IRestResponse<User> response = client.Get<User>(request);
            CheckForError(response);
            return response.Data;
        }

        public IList<User> GetUsersByUsername(string username)
        {
            RestRequest request = new RestRequest($"user?username={username}");
            IRestResponse<IList<User>> response = client.Get<IList<User>>(request);
            CheckForError(response);
            return response.Data;
        }

        public IList<User> GetUsersByAccountId(int accountId)
        {
            RestRequest request = new RestRequest($"user?accountId={accountId}");
            IRestResponse<IList<User>> response = client.Get<IList<User>>(request);
            CheckForError(response);
            return response.Data;
        }

        public IList<User> GetUsers()
        {
            RestRequest request = new RestRequest($"user");
            IRestResponse<IList<User>> response = client.Get<IList<User>>(request);
            CheckForError(response);
            return response.Data;
        }

        public Transfer CreateTransfer(Transfer newTransfer)
        {
            RestRequest request = new RestRequest("transfers");
            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            CheckForError(response);
            return response.Data;
        }







    }
}
