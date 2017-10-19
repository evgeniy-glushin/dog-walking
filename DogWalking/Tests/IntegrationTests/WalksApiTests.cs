using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Web;

namespace Tests.IntegrationTests
{
    [TestFixture]
    class WalksApiTests
    {
        // 1. Every week day has two walks - one in the morning and the second one in the evening
        // 2. Make sure the are no walks on the weekend

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
        public async Task Build_ensure_only_week_days_are_booked()
        {
            // Act
            var payload = new { dateFrom = new DateTime(2117, 10, 18), dateTo = new DateTime(2117, 10, 30) };
            StringContent json = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(BuildWalksUri, json);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createdWalks = JsonConvert.DeserializeObject<List<Walk>>(responseString);

            // Assert
            bool isWeekDaysOnly = createdWalks.Select(w => w.StartDateTime.DayOfWeek).All(IsWeekDay);
            Assert.True(isWeekDaysOnly);

            bool IsWeekDay(DayOfWeek dayOfWeek) =>
                dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
        }

        [Test]
        public async Task Build_ensure_2_walks_a_day()
        {
            // Arrange
            var payload = new { dateFrom = new DateTime(2117, 10, 18), dateTo = new DateTime(2117, 10, 30) };
            StringContent json = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(BuildWalksUri, json);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            var createdWalks = JsonConvert.DeserializeObject<List<Walk>>(responseString);
            var walksPerDay = createdWalks
                .GroupBy(w => w.StartDateTime.Date,
                              (key, g) => new { key, count = g.Count() })
                .Where(x => x.count != 2);

            Assert.AreEqual(0, walksPerDay.Count());
        }
        
        [Test]
        public async Task Build_ensure_one_walk_in_morning_and_one_in_evening()
        {
            // TODO
        }
    }
}