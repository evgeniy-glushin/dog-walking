using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web;
using static Tests.IntegrationTests.WalksHelper;

namespace Tests.IntegrationTests
{
    [TestFixture]
    class RevenueApiTests
    {
        private const string BuildWalksUri = "/api/v1/walks/build";

        private TestServer _server;
        private HttpClient _client;

        [SetUp]
        public void BeforeEach()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Test]
        public async Task Get_ensure_success_status_code()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/Revenue/");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task Calc_revenue_per_day_shouldnt_be_lower_than_walk_price()
        {
            // Arrange
            var bookedWalks = await CreateWalks(_client);

            // Act
            var response = await _client.GetAsync("/api/v1/Revenue/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var revenueReport = JsonConvert.DeserializeObject<List<RevenueReport>>(responseString);

            // Assert
            var isValid = bookedWalks.All(RevenueHigherOrEqual);
            Assert.True(isValid);

            bool RevenueHigherOrEqual(Walk walk)
            {
                var revenue = revenueReport
                    .FirstOrDefault(r => r.Date.Date == walk.StartDateTime.Date);

                return revenue?.Total >= walk.Price;
            }
        }

        [Test]
        public async Task Calc_no_revenue_when_no_walks()
        {
            // Arrange
            var bookedWalks = await CreateWalks(_client);

            // Act
            var response = await _client.GetAsync("/api/v1/Revenue/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var revenue = JsonConvert.DeserializeObject<List<RevenueReport>>(responseString);

            // Assert
            var itemsWhereTotalIsntZero = revenue
                .Where(r => NoWalks(r.Date) && r.Total != 0);

            Assert.AreEqual(0, itemsWhereTotalIsntZero.Count());

            bool NoWalks(DateTime date) =>
                bookedWalks.All(w => w.StartDateTime.Date != date.Date);
        }

        [Test]
        public async Task Calc_should_sum_prices_of_walks_per_day()
        {
            // TODO
        }
    }
}
