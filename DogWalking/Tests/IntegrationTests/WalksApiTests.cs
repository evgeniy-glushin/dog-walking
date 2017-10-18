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

        private const string BuildWalksUri = "/api/walks/build";
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
            var payload = new { dateFrom = new DateTime(2117, 10, 18), dateTo = new DateTime(2117, 10, 30) };
            StringContent json = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(BuildWalksUri, json);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createdWalks = JsonConvert.DeserializeObject<List<Walk>>(responseString);

            // TODO: get this working

            // Assert
            var group = createdWalks
                .GroupBy(w => w.StartDateTime.Date,
                              (key, g) => new { key, count = g.Count() })
                .Where(x => x.count != 2);

            Assert.AreEqual(0, group.Count());
        }
        
        [Test]
        public async Task Build_ensure_one_walk_in_morning_and_one_in_evening()
        {
            // TODO
        }
    }
}

// TODO: might be useful for integration tests
//[DogPacksWithDogs]
//public void Build_wolks_Monday_through_Friday(List<DogPack> dogPacks)
//{
//    // Arrange
//    _weeklySchedulePayload.DogPacks = dogPacks;

//    // Act
//    var walks = _weeklyScheduleBuilder.Build(_weeklySchedulePayload);

//    // Assert
//    var isWeekDays = walks.All(w => IsWeekDay(w.StartDateTime.DayOfWeek));
//    Assert.True(isWeekDays);

//    bool IsWeekDay(DayOfWeek dayOfWeek) =>
//        dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday;
//}


//public static Arbitrary<WorkingDayTimeBounds> WorkingDayTimeBounds() =>
// Arb.Default
//    .Derive<WorkingDayTimeBounds>()
//    .Filter(tb => {
//        return tb.DayStartsAt >= TimeSpan.FromHours(7) && tb.DayStartsAt <= TimeSpan.FromHours(11) &&
//                  tb.DayEndsAt >= TimeSpan.FromHours(15) && tb.DayEndsAt <= TimeSpan.FromHours(19);
//    });

//class WorkingDayTimeBounds
//{
//    public TimeSpan DayStartsAt { get; set; }
//    public TimeSpan DayEndsAt { get; set; }
//}
