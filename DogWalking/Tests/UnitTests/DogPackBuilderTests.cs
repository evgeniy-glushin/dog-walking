using FsCheck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Web.Models;
using Web.Services;

namespace Tests.UnitTests
{
    // TODO: test wrong inputs
    [TestFixture]
    class DogPackBuilderTests
    {
        [MaxPackSize]
        public void Build_all_dogs_in_pack_should_have_the_same_type(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = maxPackSize };
            var smallConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = maxPackSize };
            var largeAggressiveConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = 1, IsAggressive = true };
            var smallAggressiveConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveConfig, smallAggressiveConfig, largeConfig, smallConfig });

            // Assert
            var isSameType = dogPacks.All(dp => IsSameType(dp.Dogs));
            Assert.True(isSameType);

            bool IsSameType(IEnumerable<Dog> dogsInPack)
            {
                var first = dogsInPack.FirstOrDefault();
                return dogsInPack.All(d => d.Size == first.Size && d.IsAggressive == first.IsAggressive);
            }
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_small_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var smallConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = maxPackSize };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { smallConfig });

            // Assert
            var smallNotAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Small && !d.IsAggressive);
            var expectedPacksCount = smallNotAggressiveDogsCount > 0 ? smallNotAggressiveDogsCount / smallConfig.MaxPackCount + 1 : 0;
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_large_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = maxPackSize };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeConfig });

            // Assert
            var largeNotAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Large && !d.IsAggressive);
            var expectedPacksCount = largeNotAggressiveDogsCount > 0 ? largeNotAggressiveDogsCount / largeConfig.MaxPackCount + 1 : 0;
            var actualPacksCount = dogPacks.Count();
            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_aggressive_dogs(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeAggressiveConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = maxPackSize, IsAggressive = true };
            var smallAggressiveConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = maxPackSize, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveConfig, smallAggressiveConfig });

            // Assert
            var largeAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Large && d.IsAggressive);
            var smallAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Small && d.IsAggressive);
            var expectedPacksCount = (largeAggressiveDogsCount > 0 ? largeAggressiveDogsCount / largeAggressiveConfig.MaxPackCount + 1 : 0) +
                                     (smallAggressiveDogsCount > 0 ? smallAggressiveDogsCount / smallAggressiveConfig.MaxPackCount + 1 : 0);
            var actualPacksCount = dogPacks.Count();

            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [MaxPackSize]
        public void Build_should_build_right_number_of_all_kinds_of_dog(List<Dog> dogs, byte maxPackSize)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = maxPackSize };
            var smallConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = maxPackSize };
            var largeAggressiveConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = 1, IsAggressive = true };
            var smallAggressiveConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = 1, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveConfig, smallAggressiveConfig, largeConfig, smallConfig });

            // Assert
            var largeAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Large && d.IsAggressive);
            var smallAggressiveDogsCount = dogs.Count(d => d.Size == DogSize.Small && d.IsAggressive);
            var largeDogsCount = dogs.Count(d => d.Size == DogSize.Large && !d.IsAggressive);
            var smallDogsCount = dogs.Count(d => d.Size == DogSize.Small && !d.IsAggressive);

            var expectedPacksCount = (largeAggressiveDogsCount > 0 ? largeAggressiveDogsCount / largeAggressiveConfig.MaxPackCount + 1 : 0) +
                                     (smallAggressiveDogsCount > 0 ? smallAggressiveDogsCount / smallAggressiveConfig.MaxPackCount + 1 : 0) +
                                     (largeDogsCount > 0 ? largeDogsCount / largeConfig.MaxPackCount + 1 : 0) +
                                     (smallDogsCount > 0 ? smallDogsCount / smallConfig.MaxPackCount + 1 : 0);
            var actualPacksCount = dogPacks.Count();

            Assert.AreEqual(expectedPacksCount, actualPacksCount);
        }

        [FsCheck.NUnit.Property]
        public void Build_should_produce_no_packs_when_MaxPackCount_is_0(List<Dog> dogs)
        {
            // Arrange
            var dpb = new DogPackBuilder();
            var largeConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = 0 };
            var smallConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = 0 };
            var largeAggressiveConfig = new DogPackConfig { DogSize = DogSize.Large, MaxPackCount = 0, IsAggressive = true };
            var smallAggressiveConfig = new DogPackConfig { DogSize = DogSize.Small, MaxPackCount = 0, IsAggressive = true };

            // Act
            var dogPacks = dpb.Build(dogs, new[] { largeAggressiveConfig, smallAggressiveConfig, largeConfig, smallConfig });

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