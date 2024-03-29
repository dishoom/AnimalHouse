﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AnimalHouse.BusinessLogic;
using AnimalHouse.Data;
using AnimalHouse.Model;
using AnimalHouseAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;

namespace AnimalHouseAPI.Tests
{
    [TestClass]
    public class KennelControllerTests
    {
        private List<Kennel> _kennelData;
        private List<Animal> _animalData;
        private IQueryable<Kennel> _kennelDataQueryable;
        private IQueryable<Animal> _animalDataQueryable;
        private Mock<DbSet<Kennel>> _mockKennelDbSet;
        private Mock<DbSet<Animal>> _mockAnimalDbSet;
        private Mock<AnimalHouseDbContext> _mockContext;

        [TestInitialize]
        public void Setup()
        {
            _kennelData = new List<Kennel>()
            {
                new Kennel {kennelId = 1, name = "small", maxLimit = 16, minAnimalSize = 0, maxAminalSize = 20},
                new Kennel {kennelId = 2, name = "medium", maxLimit = 10, minAnimalSize = 20, maxAminalSize = 50},
                new Kennel {kennelId = 3, name = "large", maxLimit = 8, minAnimalSize = 50, maxAminalSize = int.MaxValue}
            };

            _animalData = new List<Animal>()
            {
                new Animal() {animalId = 1, name = "Fido", type = "Dog", sizeInLbs = 12, kennelId = 1, Kennel=_kennelData.Where(k=>k.kennelId == 1).First() },
                new Animal() {animalId = 2, name = "Mr. Whiskers", type = "Cat", sizeInLbs = 3, kennelId = 1, Kennel=_kennelData.Where(k=>k.kennelId == 1).First() },
                new Animal() {animalId = 3, name = "Spot", type = "Dog", sizeInLbs = 23, kennelId = 2, Kennel=_kennelData.Where(k=>k.kennelId == 2).First() },
                new Animal() {animalId = 4, name = "Stretch", type = "Giraffe", sizeInLbs = 1800, kennelId = 3, Kennel=_kennelData.Where(k=>k.kennelId == 3).First() },
                new Animal() {animalId = 5, name = "Mr. Ed", type = "Horse", sizeInLbs = 920, kennelId = 3, Kennel=_kennelData.Where(k=>k.kennelId == 3).First() },
            };

            _kennelDataQueryable = _kennelData.AsQueryable();
            _animalDataQueryable = _animalData.AsQueryable();

            _mockKennelDbSet = new Mock<DbSet<Kennel>>();
            _mockAnimalDbSet = new Mock<DbSet<Animal>>();

            _mockKennelDbSet.As<IDbAsyncEnumerable<Kennel>>().Setup(k => k.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Kennel>(_kennelDataQueryable.GetEnumerator()));
            _mockKennelDbSet.As<IQueryable<Kennel>>().Setup(k => k.Provider).Returns(new TestDbAsyncQueryProvider<Kennel>(_kennelDataQueryable.Provider));
            _mockKennelDbSet.As<IQueryable<Kennel>>().Setup(k => k.Expression).Returns(_kennelDataQueryable.Expression);
            _mockKennelDbSet.As<IQueryable<Kennel>>().Setup(k => k.ElementType).Returns(_kennelDataQueryable.ElementType);
            _mockKennelDbSet.As<IQueryable<Kennel>>().Setup(k => k.GetEnumerator()).Returns(_kennelDataQueryable.GetEnumerator());

            _mockAnimalDbSet.As<IDbAsyncEnumerable<Animal>>().Setup(k => k.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Animal>(_animalDataQueryable.GetEnumerator()));
            _mockAnimalDbSet.As<IQueryable<Animal>>().Setup(k => k.Provider).Returns(new TestDbAsyncQueryProvider<Animal>(_animalDataQueryable.Provider));
            _mockAnimalDbSet.As<IQueryable<Animal>>().Setup(k => k.Expression).Returns(_animalDataQueryable.Expression);
            _mockAnimalDbSet.As<IQueryable<Animal>>().Setup(k => k.ElementType).Returns(_animalDataQueryable.ElementType);
            _mockAnimalDbSet.As<IQueryable<Animal>>().Setup(k => k.GetEnumerator()).Returns(_animalDataQueryable.GetEnumerator());

            _mockKennelDbSet.Setup(k => k.Add(It.IsAny<Kennel>())).Callback<Kennel>(_kennelData.Add);
            _mockAnimalDbSet.Setup(a => a.Add(It.IsAny<Animal>())).Callback<Animal>(_animalData.Add);

            _mockContext = new Mock<AnimalHouseDbContext>();
            _mockContext.Setup(k => k.Kennels).Returns(_mockKennelDbSet.Object);
            _mockContext.Setup(k => k.Animals).Returns(_mockAnimalDbSet.Object);

            _mockContext.Setup(k => k.Kennels.Remove(It.IsAny<Kennel>())).Callback<Kennel>(k => _kennelData.Remove(k));
            _mockContext.Setup(a => a.Animals.Remove(It.IsAny<Animal>())).Callback<Animal>(a => _animalData.Remove(a));
        }

        [TestMethod]
        public async Task GetKennelReportTestAsync()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var kennelController = new KennelController(kennelProcessor);

            kennelController.Request = new HttpRequestMessage();
            kennelController.Configuration = new HttpConfiguration();

            var response = await kennelController.ReportAsync();
            
            List<KennelAnimals> kennels;

            Assert.IsTrue(response.TryGetContentValue(out kennels));
            Assert.AreEqual(3, kennels.Count);
        }
    }
}
