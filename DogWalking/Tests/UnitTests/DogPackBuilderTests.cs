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
    // TODO: test wrong inputs
    [TestFixture]
    class DogPackBuilderTests
    {
        [MaxPackSize]
        public void Build_every_dog_should_be_in_a_pack(List<Dog> dogs, byte maxPackSize)
        {            
            // Arrange
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize };
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize };
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 1, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate, largeRate, smallRate }).ToList();

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
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize };
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize };
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 1, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate, largeRate, smallRate }).ToList();

            // Assert
            Assert.AreEqual(0, dogPacks.Count(dp => dp.Dogs.Count == 0));
        }

        [MaxPackSize]
        public void Build_one_dog_shouldnt_be_in_few_packs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize };
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize };
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 1, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate, largeRate, smallRate });

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
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize };
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize };
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 1, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate, largeRate, smallRate });

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
            var dpb = new DogPackBuilder();
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { smallRate });

            // Assert
            var smallNotAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Small && !d.IsAggressive);
            var expectedPacksCount = smallNotAggressiveDogsCount > 0 ? Math.Ceiling((decimal)smallNotAggressiveDogsCount / smallRate.MaxPackSize) : 0;
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_large_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeRate });

            // Assert
            var largeNotAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Large && !d.IsAggressive);
            var expectedPacksCount = largeNotAggressiveDogsCount > 0 ? Math.Ceiling((decimal)largeNotAggressiveDogsCount / largeRate.MaxPackSize) : 0;
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_aggressive_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate });

            // Assert
            var largeAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Large && d.IsAggressive);
            var smallAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Small && d.IsAggressive);
            var expectedPacksCount = (largeAggressiveDogsCount > 0 ? Math.Ceiling((decimal)largeAggressiveDogsCount / largeAggressiveRate.MaxPackSize) : 0) +
                                     (smallAggressiveDogsCount > 0 ? Math.Ceiling((decimal)smallAggressiveDogsCount / smallAggressiveRate.MaxPackSize) : 0);
            var actualPacksCount = dogPacks.Count();

            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_all_kinds_of_dog(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = maxPackSize };
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = maxPackSize };
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 1, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate, largeRate, smallRate });

            // Assert
            var largeAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Large && d.IsAggressive);
            var smallAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Small && d.IsAggressive);
            var largeDogsCount = dogs.Count(d => d.Size == DogSize.Large && !d.IsAggressive);
            var smallDogsCount = dogs.Count(d => d.Size == DogSize.Small && !d.IsAggressive);

            var expectedPacksCount = (largeAggressiveDogsCount > 0 ? Math.Ceiling((decimal)largeAggressiveDogsCount / largeAggressiveRate.MaxPackSize) : 0) +
                                     (smallAggressiveDogsCount > 0 ? Math.Ceiling((decimal)smallAggressiveDogsCount / smallAggressiveRate.MaxPackSize) : 0) +
                                     (largeDogsCount > 0 ? Math.Ceiling((decimal)largeDogsCount / largeRate.MaxPackSize) : 0) +
                                     (smallDogsCount > 0 ? Math.Ceiling((decimal)smallDogsCount / smallRate.MaxPackSize) : 0);
            var actualPacksCount = dogPacks.Count();

            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [FsCheck.NUnit.Property]
        public void Build_should_produce_no_packs_when_MaxPackCount_is_0(List<Dog> dogs)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 0 };
            var smallRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 0 };
            var largeAggressiveRate = new PriceRate { DogSize = DogSize.Large, MaxPackSize = 0, IsAggressive = true };
            var smallAggressiveRate = new PriceRate { DogSize = DogSize.Small, MaxPackSize = 0, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveRate, smallAggressiveRate, largeRate, smallRate });

            // Assert        
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(0, actualPacksCount);
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
    }
}