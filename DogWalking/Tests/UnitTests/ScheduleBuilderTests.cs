using Domain.Models;
using FsCheck;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Services;

namespace Tests.UnitTests
{
    [TestFixture]
    class WeeklyScheduleBuilderTests
    {
        private WalksWeekdaysBuilder _weeklyScheduleBuilder;

        [SetUp]
        public void BeforeEach()
        {
            _weeklyScheduleBuilder = new WalksWeekdaysBuilder();
        }

        [ScheduleBulder]
        void Build_wolks_should_be_scheduled_at_given_days_and_time_only(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
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

            bool IsMatchWorkingHours(DateTime wolkingTime) =>
                payload.WorkingDays
                .FirstOrDefault(x => x.DayOfWeek == wolkingTime.DayOfWeek)
                ?.WorkingHours.Any(wh => wh == wolkingTime.TimeOfDay) ?? false;
        }

        [ScheduleBulder]
        void Build_the_wolks_with_empty_dog_packs_shouldnt_be_scheduled(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
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
        void Build_wolking_datetime_should_be_between_given_DateFrom_and_DateTo(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
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
            var isInTimeBoundary = walks.All(w =>
                w.StartDateTime >= payload.DateFrom &&
                w.StartDateTime <= payload.DateTo);
            Assert.True(isInTimeBoundary);
        }

        [ScheduleBulder]
        void Build_should_get_no_walks_when_wrong_input(List<DogPack> dogPacks)
        {
            var walks = _weeklyScheduleBuilder.Build(null);
            Assert.AreEqual(0, walks.Count(), "should get no walks when input is NULL");

            walks = _weeklyScheduleBuilder.Build(new WeeklySchedulePayload { });
            Assert.AreEqual(0, walks.Count(), "should get no walks when input is empty");
        }

        [ScheduleBulder]
        void Build_wolking_time_should_be_between_given_DayStartsAt_and_DayEndsAt(List<DogPack> dogPacks, ScheduleTimeBounds scheduleBounds)
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
            var isInTimeBoundary = walks.All(w =>
                w.StartDateTime.TimeOfDay >= payload.DayStartsAt &&
                w.StartDateTime.TimeOfDay <= payload.DayEndsAt - w.Duration);
            Assert.True(isInTimeBoundary);
        }

        [ScheduleBulder]
        void Build_given_payload_should_schedule_8_wolks()
        {
            // Arrange
            var defaultPayload = new WeeklySchedulePayload
            {
                DogPacks = new List<DogPack> { new DogPack { Dogs = new List<Dog> { new Dog() } } },
                DateFrom = new DateTime(2017, 10, 14),
                DateTo = new DateTime(2017, 10, 21),
                DayStartsAt = TimeSpan.FromHours(10),
                DayEndsAt = TimeSpan.FromHours(17),
                WalkDuration = TimeSpan.FromHours(1),
                Now = new DateTime(2017, 10, 16) + TimeSpan.FromHours(12)
            };

            var payload = new WeeklySchedulePayloadBuilder(defaultPayload)
               .SetupWeekDays()
               .Build();           

            // Act
            var walks = _weeklyScheduleBuilder.Build(payload).ToList();

            // Assert            
            Assert.AreEqual(8, walks.Count);
        }

        private class SchedulePayload
        {
            public static Arbitrary<DogPack> DogPack() =>
                Arb.Default
                   .Derive<DogPack>()
                   .Filter(dp => dp.Dogs.Any());

            public static Arbitrary<ScheduleTimeBounds> ScheduleTimeBounds() =>
                Arb.Default
                   .Derive<ScheduleTimeBounds>()
                   .Filter(tb => tb.DateFrom <= tb.DateTo &&
                                 tb.DateFrom <= tb.Now &&
                                 tb.DateTo >= tb.Now);
        }

        private class ScheduleBulderAttribute : FsCheck.NUnit.PropertyAttribute
        {
            public ScheduleBulderAttribute() =>
                Arbitrary = new[] { typeof(SchedulePayload) };
        }

        private class ScheduleTimeBounds
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public DateTime Now { get; set; }
        }

        private class WeeklySchedulePayloadBuilder
        {
            WeeklySchedulePayload _payload;

            public WeeklySchedulePayloadBuilder(WeeklySchedulePayload payload = null)
            {
                _payload = payload ?? Default();
            }

            private WeeklySchedulePayload Default() =>
                 new WeeklySchedulePayload
                 {
                     DateFrom = DateTime.Now,
                     DateTo = DateTime.Now + TimeSpan.FromDays(15),
                     WorkingDays = WeekDays(),
                     DayStartsAt = WalksWeekdaysBuilder.DayStartsAt,
                     DayEndsAt = WalksWeekdaysBuilder.DayEndsAt,
                     WalkDuration = TimeSpan.FromHours(1),
                     Now = DateTime.Now
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

            public WeeklySchedulePayload Build() => _payload;            
        }
    }
}