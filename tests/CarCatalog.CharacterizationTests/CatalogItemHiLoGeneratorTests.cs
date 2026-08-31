using System;
using System.Reflection;
using CarCatalogLegacyMVC.Models;
using Xunit;

namespace CarCatalog.CharacterizationTests
{
    /// <summary>
    /// Characterization tests for <see cref="CatalogItemHiLoGenerator"/>. The hi block is
    /// fetched with <c>SELECT NEXT VALUE FOR catalog_hilo</c>, which needs SQL Server, so
    /// the seeded state is set up directly and only the lo allocation is exercised here.
    /// </summary>
    public class CatalogItemHiLoGeneratorTests
    {
        private const int HiLoIncrement = 10;

        private static void SeedHiBlock(CatalogItemHiLoGenerator generator, int hi)
        {
            SetField(generator, "sequenceId", hi);
            SetField(generator, "remainningLoIds", HiLoIncrement - 1);
        }

        private static void SetField(CatalogItemHiLoGenerator generator, string name, int value)
        {
            var field = typeof(CatalogItemHiLoGenerator).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            field.SetValue(generator, value);
        }

        private static int GetField(CatalogItemHiLoGenerator generator, string name)
        {
            var field = typeof(CatalogItemHiLoGenerator).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            return (int)field.GetValue(generator);
        }

        [Fact]
        public void GetNextSequenceValue_HandsOutNineConsecutiveIdsAfterFetchingAHiBlock()
        {
            var generator = new CatalogItemHiLoGenerator();
            SeedHiBlock(generator, 1000);

            for (var i = 1; i < HiLoIncrement; i++)
            {
                // No database is touched while lo ids remain in the current block.
                Assert.Equal(1000 + i, generator.GetNextSequenceValue(null));
            }

            Assert.Equal(0, GetField(generator, "remainningLoIds"));
        }

        [Fact]
        public void GetNextSequenceValue_GoesBackToTheDatabaseOnceTheBlockIsExhausted()
        {
            var generator = new CatalogItemHiLoGenerator();
            SeedHiBlock(generator, 1000);

            for (var i = 1; i < HiLoIncrement; i++)
            {
                generator.GetNextSequenceValue(null);
            }

            Assert.Throws<NullReferenceException>(() => generator.GetNextSequenceValue(null));
        }

        [Fact]
        public void GetNextSequenceValue_RequiresADatabaseForTheFirstValue()
        {
            var generator = new CatalogItemHiLoGenerator();

            Assert.Throws<NullReferenceException>(() => generator.GetNextSequenceValue(null));
            Assert.Equal(0, GetField(generator, "remainningLoIds"));
        }
    }
}
