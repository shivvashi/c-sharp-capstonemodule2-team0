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
            RestRequest request = new RestRequest($"/account/{userId}/transfers");
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

        public string GetFromUserByAccountId(int id)
        {
            RestRequest request = new RestRequest($"transfers/from/{id}");
            IRestResponse<string> response = client.Get<string>(request);
            CheckForError(response);
            return response.Data;
        }

        public string GetToUserByAccountId(int id)
        {
            RestRequest request = new RestRequest($"transfers/to/{id}");
            IRestResponse<string> response = client.Get<string>(request);
            CheckForError(response);
            return response.Data;
        }





    }
}
