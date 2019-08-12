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
    public class AnimalProcessorTests
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
                new Kennel {id = 1, name = "small", maxLimit = 16, minAnimalSize = 0, maxAminalSize = 20},
                new Kennel {id = 2, name = "medium", maxLimit = 10, minAnimalSize = 20, maxAminalSize = 50},
                new Kennel {id = 3, name = "large", maxLimit = 8, minAnimalSize = 50, maxAminalSize = int.MaxValue}
            };

            _animalData = new List<Animal>()
            {
                new Animal() {id = 1, name = "Fido", type = "Dog", sizeInLbs = 12, kennelId = 1, Kennel=_kennelData.Where(k=>k.id == 1).First() },
                new Animal() {id = 2, name = "Mr. Whiskers", type = "Cat", sizeInLbs = 3, kennelId = 1, Kennel=_kennelData.Where(k=>k.id == 1).First() },
                new Animal() {id = 3, name = "Spot", type = "Dog", sizeInLbs = 23, kennelId = 2, Kennel=_kennelData.Where(k=>k.id == 2).First() },
                new Animal() {id = 4, name = "Stretch", type = "Giraffe", sizeInLbs = 1800, kennelId = 3, Kennel=_kennelData.Where(k=>k.id == 3).First() },
                new Animal() {id = 5, name = "Mr. Ed", type = "Horse", sizeInLbs = 920, kennelId = 3, Kennel=_kennelData.Where(k=>k.id == 3).First() },
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
        public async Task AddAnimalToShelterAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            var animalToAdd = new Animal() { name = "Bingo", type = "Dog", sizeInLbs = 26 };

            var wasAminalAdded = await animalProcessor.AddAnimalToShelterAsync(animalToAdd);

            Assert.IsTrue(wasAminalAdded);
        }

        [TestMethod]
        public async Task RejectAnimalIfKennelFullAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });
            _animalData.Add(new Animal { kennelId = 2 });

            var animalToAdd = new Animal() { name = "Bingo", type = "Dog", sizeInLbs = 26 };

            var wasAminalAdded = await animalProcessor.AddAnimalToShelterAsync(animalToAdd);

            Assert.IsFalse(wasAminalAdded);
        }

        [TestMethod]
        public async Task RemoveAnimalByIdAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            var wasAnimalRemoved = await animalProcessor.RemoveAnimalById(1);
            Assert.IsTrue(wasAnimalRemoved);

            var shouldBeNull = _animalData.Where(a => a.id == 1).FirstOrDefault();
            Assert.IsTrue(shouldBeNull == null);
        }

        [TestMethod]
        public async Task RemoveAnimalByNameAndTypeAndSizeAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            var wasAnimalRemoved = await animalProcessor.RemoveAnimalByNameAndTypeAndSize("Fido", "Dog", 12);
            Assert.IsTrue(wasAnimalRemoved);

            var shouldBeNull = _animalData.Where(a => a.id == 1).FirstOrDefault();
            Assert.IsTrue(shouldBeNull == null);
        }

        [TestMethod]
        public async Task NoAnimalLeftBehindAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            //remove medium and large kennels, then try to restucture
            _kennelData.Remove(_kennelData.Where(k => k.id == 2).First());
            _kennelData.Remove(_kennelData.Where(k => k.id == 3).First());

            var wasRestructureSuccess = await animalProcessor.ReorganizeAnimalsToAppropriateKennels();

            Assert.IsFalse(wasRestructureSuccess);
        }

        [TestMethod]
        public async Task RestructureFailsByOvercapacityAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            //set capacity for each kennel to 1, which should force the business logic to fail
            _kennelData.ForEach(k => k.maxLimit = 1);

            var wasRestructureSuccess = await animalProcessor.ReorganizeAnimalsToAppropriateKennels();

            Assert.IsFalse(wasRestructureSuccess);
        }

        [TestMethod]
        public async Task RestructureAsyncTest()
        {
            var kennelProcessor = new KennelProcessor(_mockContext.Object);
            var animalProcessor = new AnimalProcessor(_mockContext.Object, kennelProcessor);

            //test that the count of animals in small kennel is 2 before the restructure
            var smallAnimalsBeforeRestructure = await animalProcessor.GetAnimalsInKennel(1);

            //reorganizing the kennel sizes
            //increase small size definition to be less than than 30 lbs
            //increase medium size definition to be greater than 30 lbs
            _kennelData.Where(k => k.name == "small").ToList().ForEach(k => k.maxAminalSize = 30);
            _kennelData.Where(k => k.name == "medium").ToList().ForEach(k => k.minAnimalSize = 30);          

            var wasRestructureSuccess = await animalProcessor.ReorganizeAnimalsToAppropriateKennels();
            var smallAnimalsAfterRestructure = await animalProcessor.GetAnimalsInKennel(1);

            Assert.IsTrue(wasRestructureSuccess);
            Assert.IsTrue(smallAnimalsBeforeRestructure.Count == 2);
            Assert.IsTrue(smallAnimalsAfterRestructure.Count == 3);
        }

    }
}
