using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using AnimalHouse.BusinessLogic;
using AnimalHouse.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AnimalHouse.Test
{
    [TestClass]
    public class KennelProcessorTests
    {
        [TestMethod]
        public async Task GetCorrectKennelSizeForAnimalAsync()
        {
            var kennelData = new List<Kennel>()
            {
                new Kennel {id = 1, name = "small", maxLimit = 16, minAnimalSize = 0, maxAminalSize = 20},
                new Kennel {id = 2, name = "medium", maxLimit = 10, minAnimalSize = 20, maxAminalSize = 50},
                new Kennel {id = 3, name = "large", maxLimit = 8, minAnimalSize = 50, maxAminalSize = int.MaxValue}
            }.AsQueryable();


            var mockSet = new Mock<DbSet<Kennel>>();
            mockSet.As<IDbAsyncEnumerable<Kennel>>()
                .Setup(k => k.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Kennel>(kennelData.GetEnumerator()));

            mockSet.As<IQueryable<Kennel>>()
                .Setup(k => k.Provider)
                .Returns(new TestDbAsyncQueryProvider<Kennel>(kennelData.Provider));

            mockSet.As<IQueryable<Kennel>>().Setup(k => k.Expression).Returns(kennelData.Expression);
            mockSet.As<IQueryable<Kennel>>().Setup(k => k.ElementType).Returns(kennelData.ElementType);
            mockSet.As<IQueryable<Kennel>>().Setup(k => k.GetEnumerator()).Returns(kennelData.GetEnumerator());


            var mockContext = new Mock<AnimalHouseDbContext>();
            mockContext.Setup(k => k.Kennels).Returns(mockSet.Object);

            var kennelProcessor = new KennelProcessor(mockContext.Object);

            var smallKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(12);
            var mediumKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(35);
            var largeKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(64);

            Assert.IsTrue(smallKennel.name == "small");
            Assert.IsTrue(mediumKennel.name == "medium");
            Assert.IsTrue(largeKennel.name == "large");

            //edge cases
            smallKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(.001);
            Assert.IsTrue(smallKennel.name == "small");
            smallKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(20);
            Assert.IsTrue(smallKennel.name == "small");

            mediumKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(20.01);
            Assert.IsTrue(mediumKennel.name == "medium");
            mediumKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(50);
            Assert.IsTrue(mediumKennel.name == "medium");

            largeKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(50.01);
            Assert.IsTrue(largeKennel.name == "large");
            largeKennel = await kennelProcessor.GetKennelByAnimalSizeAsync(99999);
            Assert.IsTrue(largeKennel.name == "large");

        }
    }
}
