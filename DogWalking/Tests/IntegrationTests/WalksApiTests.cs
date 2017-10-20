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
using static Tests.IntegrationTests.WalksHelper;

namespace Tests.IntegrationTests
{
    [TestFixture]
    class WalksApiTests
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
            var response = await _client.GetAsync("/api/v1/walks/get");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task Build_ensure_success_status_code()
        {
            // Act
            var createdWalks = await CreateWalks(_client);
        }

        [Test]
        public async Task Build_ensure_only_week_days_are_booked()
        {
            // Act
            var createdWalks = await CreateWalks(_client);

            // Assert
            bool isWeekDaysOnly = createdWalks.Select(w => w.StartDateTime.DayOfWeek).All(IsWeekDay);
            Assert.True(isWeekDaysOnly);

            bool IsWeekDay(DayOfWeek dayOfWeek) =>
                dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
        }

        [Test]
        public async Task Build_ensure_2_walks_a_day()
        {
            // Act
            var createdWalks = await CreateWalks(_client);

            // Assert
            var wrongWalks = createdWalks
                .GroupBy(w => w.StartDateTime.Date,
                              (key, g) => new { key, count = g.Count() })
                .Where(x => x.count != 2);

            Assert.AreEqual(0, wrongWalks.Count());
        }
        
        [Test]
        public async Task Build_ensure_one_walk_in_morning_and_one_in_evening()
        {
            // TODO
        }
    }
}