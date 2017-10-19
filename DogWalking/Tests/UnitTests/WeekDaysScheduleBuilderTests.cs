using Domain.Models;
using FsCheck;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Services;
using static Tests.DogPacksHelper;

namespace Tests.UnitTests
{
    [TestFixture]
    class WeeklyScheduleBuilderTests
    {
        private WeekDaysScheduleBuilder _weeklyScheduleBuilder;

        [SetUp]
        public void BeforeEach()
        {
            _weeklyScheduleBuilder = new WeekDaysScheduleBuilder();
        }

        [ScheduleBulder]
        public void Build_walks_should_be_scheduled_at_given_days_and_time_only(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
        {
            // Arrange
            var payload = new WeeklySchedulePayloadBuilder()
                .UseDogPacks(dogPacks)
                .UseScheduleBounds(scheduleBounds)
                .SetupWeekDays()
                .Build();

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload);

            // Assert
            var isMatchWorkingHours = walks.All(w => IsMatchWorkingHours(w.StartDateTime));
            Assert.True(isMatchWorkingHours);

            bool IsMatchWorkingHours(DateTime walkingTime) =>
                payload.WorkingDays
                .FirstOrDefault(x => x.DayOfWeek == walkingTime.DayOfWeek)
                ?.WorkingHours.Any(wh => wh == walkingTime.TimeOfDay) ?? false;
        }

        [ScheduleBulder]
        public void Build_the_walks_with_empty_dog_packs_shouldnt_be_scheduled(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
        {
            // Arrange
            var payload = new WeeklySchedulePayloadBuilder()
               .UseDogPacks(dogPacks)
               .UseScheduleBounds(scheduleBounds)
               .SetupWeekDays()
               .Build();

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload);

            // Assert
            var allWithDogs = walks.All(w => w.DogPack?.Dogs.Any() ?? false);
            Assert.True(allWithDogs);
        }

        [ScheduleBulder]
        public void Build_walking_datetime_should_be_between_given_DateFrom_and_DateTo(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
        {
            // Arrange
            var payload = new WeeklySchedulePayloadBuilder()
               .UseDogPacks(dogPacks)
               .UseScheduleBounds(scheduleBounds)
               .SetupWeekDays()
               .Build();

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload).ToList();

            // Assert
            var outOfTimeBoundsWalks = walks.Where(w =>
                w.StartDateTime < payload.DateFrom &&
                w.StartDateTime > payload.DateTo).ToList();

            Assert.AreEqual(0, outOfTimeBoundsWalks.Count);
        }

        [ScheduleBulder]
        public void Build_walk_price_divided_on_dog_price_per_walk_should_equal_dogs_count_in_pack(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
        {
            // Arrange
            var payload = new WeeklySchedulePayloadBuilder()
               .UseDogPacks(dogPacks)
               .UseScheduleBounds(scheduleBounds)
               .SetupWeekDays()
               .Build();

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload).ToList();

            // Assert
            var walksWithWrongTotal = walks.Where(NotValidPrice).ToList();
            Assert.AreEqual(0, walksWithWrongTotal.Count);
            
            bool NotValidPrice(Walk walk) => 
                (walk.Price / PriceForDogPerWalk(walk)) != walk.DogPack.Dogs.Count;

            decimal PriceForDogPerWalk(Walk walk) =>
                PriceRate(GetDogPackType(walk.DogPack)).PricePerWalk;
            
            PriceRate PriceRate((DogSize size, bool isAggressive) data) =>
                payload.PriceRates.Find(pr => pr.DogSize == data.size &&
                                              pr.IsAggressive == data.isAggressive);
        }

        // TODO: refactor this test
        [ScheduleBulder]
        public void Build_should_get_no_walks_when_wrong_input(List<DogPack> dogPacks)
        {
            var walks = _weeklyScheduleBuilder.Build(null);
            Assert.AreEqual(0, walks.Count(), "should get no walks when input is NULL");

            walks = _weeklyScheduleBuilder.Build(new WeekDaysSchedulePayload { });
            Assert.AreEqual(0, walks.Count(), "should get no walks when input is empty");
        }

        [ScheduleBulder]
        public void Build_given_payload_should_schedule_8_walks()
        {
            // Arrange      
            var payload = new WeeklySchedulePayloadBuilder()
               .UseDogPacks(new List<DogPack> { new DogPack { Dogs = new List<Dog> { new Dog() { Size = DogSize.Large } } } })
               .SetupWeekDays()
               .Build();

            // TODO: refactor this
            payload.DateFrom = new DateTime(2017, 10, 14);
            payload.DateTo = new DateTime(2017, 10, 21);
            payload.WalkDuration = TimeSpan.FromHours(1);
            payload.Now = new DateTime(2017, 10, 16) + TimeSpan.FromHours(12);

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload).ToList();

            // Assert            
            Assert.AreEqual(8, walks.Count);
        }

        public class SchedulePayload
        {
            public static Arbitrary<DogPack> DogPack() =>
                Arb.Default
                   .Derive<DogPack>()
                   .Filter(dp => dp.Dogs.Any() && HaveSameType(dp.Dogs));

            public static Arbitrary<ScheduleTimeBounds> ScheduleTimeBounds() =>
                Arb.Default
                   .Derive<ScheduleTimeBounds>()
                   .Filter(tb => tb.DateFrom <= tb.DateTo &&
                                 tb.DateFrom <= tb.Now &&
                                 tb.DateTo >= tb.Now);
        }

        public class ScheduleBulderAttribute : FsCheck.NUnit.PropertyAttribute
        {
            public ScheduleBulderAttribute() =>
                Arbitrary = new[] { typeof(SchedulePayload) };
        }

        public class ScheduleTimeBounds
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public DateTime Now { get; set; }
        }

        public class WeeklySchedulePayloadBuilder
        {
            WeekDaysSchedulePayload _payload;

            public WeeklySchedulePayloadBuilder(WeekDaysSchedulePayload payload = null)
            {
                _payload = payload ?? Default();
            }

            private WeekDaysSchedulePayload Default() =>
                 new WeekDaysSchedulePayload
                 {
                     DateFrom = DateTime.Now,
                     DateTo = DateTime.Now + TimeSpan.FromDays(15),
                     WorkingDays = WeekDays(),
                     WalkDuration = TimeSpan.FromHours(1),
                     Now = DateTime.Now,
                     PriceRates = new List<PriceRate>
                     {
                         new PriceRate { DogSize = DogSize.Small, IsAggressive = false, MaxPackSize = 5, PricePerWalk = 10 },
                         new PriceRate { DogSize = DogSize.Large, IsAggressive = false, MaxPackSize = 3, PricePerWalk = 20 },
                         new PriceRate { DogSize = DogSize.Small, IsAggressive = true, MaxPackSize = 1, PricePerWalk = 55 },
                         new PriceRate { DogSize = DogSize.Large, IsAggressive = true, MaxPackSize = 1, PricePerWalk = 65 }
                     }
                 };

            private List<WorkingDay> WeekDays() =>
                Enumerable.Range((int)DayOfWeek.Monday, 5)
                .Select(day => new WorkingDay
                {
                    DayOfWeek = (DayOfWeek)day,
                    WorkingHours = new List<TimeSpan> { TimeSpan.FromHours(10), TimeSpan.FromHours(16) }
                }).ToList();

            public WeeklySchedulePayloadBuilder UseScheduleBounds(ScheduleTimeBounds bounds)
            {
                _payload.DateFrom = bounds.DateFrom;
                _payload.DateTo = bounds.DateTo;
                _payload.Now = bounds.Now;
                return this;
            }

            public WeeklySchedulePayloadBuilder UseDogPacks(List<DogPack> dogPacks)
            {
                _payload.DogPacks = dogPacks;
                return this;
            }

            public WeeklySchedulePayloadBuilder SetupWeekDays()
            {
                _payload.WorkingDays = WeekDays();
                return this;
            }

            public WeekDaysSchedulePayload Build() => _payload;
        }
    }
}