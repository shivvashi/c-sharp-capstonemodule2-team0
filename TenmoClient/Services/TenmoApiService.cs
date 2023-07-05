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
        public Account GetAccount(int id)
        {
            RestRequest request = new RestRequest($"account/{id}", DataFormat.Json);
            IRestResponse<Account> response = client.Get<Account>(request);
            CheckForError(response);
            return response.Data;

        }

        public List<Transfer> GetTransfers(int userId)
        {
            RestRequest request = new RestRequest($"transfer", DataFormat.Json);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            CheckForError(response);
            return response.Data;
        }

        public Transfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest($"transfer/{transferId}");
            IRestResponse<Transfer> response = client.Get<Transfer>(request);
            CheckForError(response);
            return response.Data;
        }

        public string GetFromUserById(int id)
        {
            RestRequest request = new RestRequest($"transfer/from/{id}");
            IRestResponse<string> response = client.Get<string>(request);
            CheckForError(response);
            return response.Data;
        }

        public string GetToUserById(int id)
        {
            RestRequest request = new RestRequest($"transfer/to/{id}");
            IRestResponse<string> response = client.Get<string>(request);
            CheckForError(response);
            return response.Data;
        }





    }
}
