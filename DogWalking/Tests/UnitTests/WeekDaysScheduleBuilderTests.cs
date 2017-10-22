using Domain.Models;
using FsCheck;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                .With(p => p.DogPacks = dogPacks)
                .With(p => p.DateFrom = scheduleBounds.DateFrom)
                .With(p => p.DateTo = scheduleBounds.DateTo)
                .With(p => p.Now = scheduleBounds.Now)
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
                .With(p => p.DogPacks = dogPacks)
                .With(p => p.DateFrom = scheduleBounds.DateFrom)
                .With(p => p.DateTo = scheduleBounds.DateTo)
                .With(p => p.Now = scheduleBounds.Now)
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
               .With(p => p.DogPacks = dogPacks)
               .With(p => p.DateFrom = scheduleBounds.DateFrom)
               .With(p => p.DateTo = scheduleBounds.DateTo)
               .With(p => p.Now = scheduleBounds.Now)
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
               .With(p => p.DogPacks = dogPacks)
               .With(p => p.DateFrom = scheduleBounds.DateFrom)
               .With(p => p.DateTo = scheduleBounds.DateTo)
               .With(p => p.Now = scheduleBounds.Now)
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

        [Test]
        public void Build_should_get_no_walks_when_wrong_input()
        {
            var walks = _weeklyScheduleBuilder.Build(null);
            Assert.AreEqual(0, walks.Count(), "should get no walks when input is NULL");

            walks = _weeklyScheduleBuilder.Build(new WeekDaysSchedulePayload { });
            Assert.AreEqual(0, walks.Count(), "should get no walks when input is empty");

            var payloadWithNoDogPacks = new WeeklySchedulePayloadBuilder()
               .With(p => p.DogPacks = null)
               .Build();

            walks = _weeklyScheduleBuilder.Build(payloadWithNoDogPacks);
            Assert.AreEqual(0, walks.Count(), "should get no walks when no dog packs");

            var payloadWithNoPriceRates = new WeeklySchedulePayloadBuilder()
              .With(p => p.PriceRates = null)
              .Build();

            walks = _weeklyScheduleBuilder.Build(payloadWithNoPriceRates);
            Assert.AreEqual(0, walks.Count(), "should get no walks when no price rates");

            var payloadWithNoWorkingDays = new WeeklySchedulePayloadBuilder()
             .With(p => p.WorkingDays = null)
             .Build();

            walks = _weeklyScheduleBuilder.Build(payloadWithNoWorkingDays);
            Assert.AreEqual(0, walks.Count(), "should get no walks when no working days");
        }

        [Test]
        public void Build_given_payload_should_schedule_9_walks()
        {
            // Arrange      
            var payload = new WeeklySchedulePayloadBuilder()
             .With(p => p.DogPacks = new List<DogPack> { new DogPack { Dogs = new List<Dog> { new Dog() { Size = DogSize.Large } } } })
             .With(p => p.DateFrom = new DateTime(2017, 10, 14))
             .With(p => p.DateTo = new DateTime(2017, 10, 21))
             .With(p => p.WalkDuration = TimeSpan.FromHours(1))
             .With(p => p.Now = new DateTime(2017, 10, 16) + TimeSpan.FromHours(12))
             .SetupWeekDays()
             .Build();

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload).ToList();

            // Assert            
            Assert.AreEqual(9, walks.Count);
        }

        public class SchedulePayload
        {
            public static Arbitrary<DogPack> DogPack() =>
                Arb.Default
                   .Derive<DogPack>()
                   .MapFilter(AdjustDogPack, dp => dp.Dogs.Any());

            public static Arbitrary<ScheduleTimeBounds> ScheduleTimeBounds() =>
                Arb.Default
                   .Derive<ScheduleTimeBounds>()
                   .MapFilter(AdjustDates, tb => true);

            /// <summary>
            /// Adjusts dog type for each dog in a pack.
            /// NOTE: significantly improves the performance of the tests.
            /// </summary>
            static DogPack AdjustDogPack(DogPack dp)
            {
                if (dp.Dogs.Any())
                    if (!HaveSameType(dp.Dogs))
                    {
                        var first = dp.Dogs.First();
                        dp.Dogs.ForEach(x =>
                             (x.Size, x.IsAggressive) = (first.Size, first.IsAggressive));
                    }

                return dp;
            }

            /// <summary>
            /// Adjusts the time boundaries.
            /// NOTE: significantly improves the performance of the tests.
            /// </summary>
            static ScheduleTimeBounds AdjustDates(ScheduleTimeBounds dates)
            {
                // set current year for all the input dates
                var dateFromDiff = DateTime.Now.Year - dates.DateFrom.Year;
                if (dateFromDiff != 0)
                    dates.DateFrom = dates.DateFrom.AddYears(dateFromDiff);

                var dateToDiff = DateTime.Now.Year - dates.DateTo.Year;
                if (dateToDiff != 0)
                    dates.DateTo = dates.DateTo.AddYears(dateToDiff);

                var nowDiff = DateTime.Now.Year - dates.Now.Year;
                if (nowDiff != 0)
                    dates.Now = dates.Now.AddYears(nowDiff);

                // adjusts the time boundaries 
                if (dates.DateFrom > dates.DateTo)
                    (dates.DateFrom, dates.DateTo) = (dates.DateTo, dates.DateFrom); // swap             

                if (dates.Now < dates.DateFrom)
                    (dates.Now, dates.DateFrom) = (dates.DateFrom, dates.Now); // swap
                else if (dates.Now > dates.DateTo)
                    (dates.Now, dates.DateTo) = (dates.DateTo, dates.Now); // swap

                return dates;
            }
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

            public WeeklySchedulePayloadBuilder With(Action<WeekDaysSchedulePayload> f)
            {
                f(_payload);
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