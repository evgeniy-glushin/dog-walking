using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tests.IntegrationTests
{
    class WalksHelper
    {
        private const string BuildWalksUri = "/api/v1/walks/build";

        public static async Task<List<Walk>> CreateWalks(HttpClient httpClient)
        {
            var payload = new { dateFrom = new DateTime(2117, 10, 18), dateTo = new DateTime(2117, 10, 30) };
            StringContent json = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(BuildWalksUri, json);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createdWalks = JsonConvert.DeserializeObject<List<Walk>>(responseString);
            return createdWalks;
        }
    }
}
