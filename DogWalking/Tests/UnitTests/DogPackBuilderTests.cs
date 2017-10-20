using Domain.Models;
using FsCheck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Web.Services;
using static Tests.DogPacksHelper;
using static Domain.Models.DogSize;
using static System.Math;

namespace Tests.UnitTests
{
    [TestFixture]
    class DogPackBuilderTests
    {
        private DogPackBuilder _builder;

        [SetUp]
        public void BeforeEach()
        {
            _builder = new DogPackBuilder();
        }

        [MaxPackSize]
        public void Build_every_dog_should_be_in_a_pack(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
                .With(Large, false, maxPackSize)
                .With(Small, false, maxPackSize)
                .With(Large, true, 1)
                .With(Small, true, 1)
                .Build();

            // Act
            var dogPacks = _builder.Build(dogs, priceRates).ToList();

            // Assert
            var missedDogs = dogs.Where(IsInDogPacks).ToList();
            Assert.AreEqual(0, missedDogs.Count);

            bool IsInDogPacks(Dog dog) =>
                !dogPacks
                    .SelectMany(p => p.Dogs)
                    .Any(d => d == dog);
        }

        [MaxPackSize]
        public void Build_shouldnt_produce_packs_with_no_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
                .With(Large, false, maxPackSize)
                .With(Small, false, maxPackSize)
                .With(Large, true, 1)
                .With(Small, true, 1)
                .Build();

            // Act
            var dogPacks = _builder.Build(dogs, priceRates).ToList();

            // Assert
            Assert.AreEqual(0, dogPacks.Count(dp => dp.Dogs.Count == 0));
        }

        [MaxPackSize]
        public void Build_one_dog_shouldnt_be_in_few_packs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
                .With(Large, false, maxPackSize)
                .With(Small, false, maxPackSize)
                .With(Large, true, 1)
                .With(Small, true, 1)
                .Build();

            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert
            var duplicatedDogs = dogs.Where(d => CountInPacks(d) > 1).ToList();
            Assert.AreEqual(0, duplicatedDogs.Count);

            int CountInPacks(Dog dog) =>
                dogPacks
                    .SelectMany(p => p.Dogs)
                    .Count(d => d == dog);
        }

        [MaxPackSize]
        public void Build_all_dogs_in_pack_should_have_the_same_type(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange           
            var priceRates = new PriceRatesBuilder()
                .With(Large, false, maxPackSize)
                .With(Small, false, maxPackSize)
                .With(Large, true, 1)
                .With(Small, true, 1)
                .Build();

            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert
            var isSameType = dogPacks
                .Select(dp => dp.Dogs)
                .All(HaveSameType);

            Assert.True(isSameType);            
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_small_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
                .With(Small, false, maxPackSize)
                .Build();
            
            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert
            var smallNotAggressiveDogsCount = dogs.Count(d => d.Size == Small && !d.IsAggressive);
            var expectedPacksCount = smallNotAggressiveDogsCount > 0 ? Ceiling((decimal)smallNotAggressiveDogsCount / maxPackSize) : 0;
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_large_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
                .With(Large, false, maxPackSize)
                .Build();
            
            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert
            var largeNotAggressiveDogsCount = dogs.Count(d => d.Size == Large && !d.IsAggressive);
            var expectedPacksCount = largeNotAggressiveDogsCount > 0 ? Ceiling((decimal)largeNotAggressiveDogsCount / maxPackSize) : 0;
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_aggressive_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
               .With(Large, true, maxPackSize)
               .With(Small, true, maxPackSize)
               .Build();
            
            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert
            var largeAggressiveDogsCount = dogs.Count(d => d.Size == Large && d.IsAggressive);
            var smallAggressiveDogsCount = dogs.Count(d => d.Size == Small && d.IsAggressive);
            var expectedPacksCount = (largeAggressiveDogsCount > 0 ? Ceiling((decimal)largeAggressiveDogsCount / maxPackSize) : 0) +
                                     (smallAggressiveDogsCount > 0 ? Ceiling((decimal)smallAggressiveDogsCount / maxPackSize) : 0);
            var actualPacksCount = dogPacks.Count();

            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_all_kinds_of_dog(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var (largePackSize, smallPackSize) = (maxPackSize, maxPackSize);
            var (largeAggressivePackSize, smallAggressivePackSize) = ((byte)1, (byte)1);

            var priceRates = new PriceRatesBuilder()
                .With(Large, false, largePackSize)
                .With(Small, false, smallPackSize)
                .With(Large, true, largeAggressivePackSize)
                .With(Small, true, smallAggressivePackSize)
                .Build();

            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert
            var largeAggressiveDogsCount = dogs.Count(d => d.Size == Large && d.IsAggressive);
            var smallAggressiveDogsCount = dogs.Count(d => d.Size == Small && d.IsAggressive);
            var largeDogsCount = dogs.Count(d => d.Size == Large && !d.IsAggressive);
            var smallDogsCount = dogs.Count(d => d.Size == Small && !d.IsAggressive);

            var expectedPacksCount = (largeAggressiveDogsCount > 0 ? Ceiling((decimal)largeAggressiveDogsCount / largeAggressivePackSize) : 0) +
                                     (smallAggressiveDogsCount > 0 ? Ceiling((decimal)smallAggressiveDogsCount / smallAggressivePackSize) : 0) +
                                     (largeDogsCount > 0 ? Ceiling((decimal)largeDogsCount / largePackSize) : 0) +
                                     (smallDogsCount > 0 ? Ceiling((decimal)smallDogsCount / smallPackSize) : 0);
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [FsCheck.NUnit.Property]
        public void Build_should_produce_no_packs_when_MaxPackSize_is_0(List<Dog> dogs)
        {
            // Arrange
            var priceRates = new PriceRatesBuilder()
                .With(Large, false, 0)
                .With(Small, false, 0)
                .With(Large, true, 0)
                .With(Small, true, 0)
                .Build();

            // Act
            var dogPacks = _builder.Build(dogs, priceRates);

            // Assert        
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(0, actualPacksCount);
        }

        [Test]
        public void Build_should_get_no_dog_packs_when_wrong_input()
        {
           // TODO
        }

        private class MaxPackSize
        {
            public static Arbitrary<byte> Byte() =>
                Arb.Default.Byte().Filter(x => x > 0 && x < 30);
        }

        private class MaxPackSizeAttribute : FsCheck.NUnit.PropertyAttribute
        {
            public MaxPackSizeAttribute() =>
                Arbitrary = new[] { typeof(MaxPackSize) };
        }

        private class PriceRatesBuilder
        {
            List<PriceRate> _priceRates;

            public PriceRatesBuilder()
            {
                _priceRates = new List<PriceRate>();
            }

            public PriceRatesBuilder With(DogSize size, bool isAggressive, byte maxPackSize)
            {
                _priceRates.Add(new PriceRate { DogSize = size, MaxPackSize = maxPackSize, IsAggressive = isAggressive });
                return this;
            }

            public List<PriceRate> Build() => _priceRates;
        }
    }
}