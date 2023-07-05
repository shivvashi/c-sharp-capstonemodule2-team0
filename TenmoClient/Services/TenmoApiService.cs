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
            RestRequest request = new RestRequest($"account/{id}");
            IRestResponse<Account> response = client.Get<Account>(request);
            CheckForError(response);
            return response.Data;

        }

    }
}
