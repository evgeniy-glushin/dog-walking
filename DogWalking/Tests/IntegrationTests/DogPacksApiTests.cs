using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web;
using static Tests.DogPacksHelper;

namespace Tests.IntegrationTests
{
    [TestFixture]
    class DogPacksApiTests
    {
        private const string BuildPacksUri = "/api/v1/dogpacks/build";
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
            var response = await _client.GetAsync("/api/v1/dogpacks/get");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task Build_ensure_success_status_code()
        {
            // Act
            var response = await _client.PostAsync(BuildPacksUri, null);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task Build_ensure_dogs_separated_into_small_large_and_aggressive_packs()
        {
            // Act
            var response = await _client.PostAsync(BuildPacksUri, null);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createdPacks = JsonConvert.DeserializeObject<List<DogPack>>(responseString);

            // Assert
            var isSameType = createdPacks
                .Select(dp => dp.Dogs)
                .All(HaveSameType);

            Assert.True(isSameType);
        }

        [Test]
        public async Task Build_ensure_every_type_of_pack_has_right_number_of_dogs()
        {
            // Arrange
            // pack sizes match with requirements large -3, small - 5, aggressive - 1
            var priceRates = new[]
            {
                new { size = DogSize.Small, isAggressive = false, number = 5 },
                new { size = DogSize.Large, isAggressive = false, number = 3 },
                new { size = DogSize.Small, isAggressive = true, number = 1 },
                new { size = DogSize.Large, isAggressive = true, number = 1 }
            };

            // Act
            var response = await _client.PostAsync(BuildPacksUri, null);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createdPacks = JsonConvert.DeserializeObject<List<DogPack>>(responseString);

            // Assert
            var isExpectedSize = createdPacks.All(HaveCorrectSize);
            Assert.True(isExpectedSize);

            bool HaveCorrectSize(DogPack pack)
            {
                var (size, isAggressive) = GetDogPackType(pack);
                var expected = priceRates.First(x => x.size == size && x.isAggressive == isAggressive);
                return pack.Dogs.Count <= expected.number;
            }            
        }
    }
}
