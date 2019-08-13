using AnimalHouse.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHouse.Data
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<AnimalHouseDbContext>
    {
        protected override void Seed(AnimalHouseDbContext context)
        {
            base.Seed(context);

            var kennelData = new List<Kennel>()
            {
                new Kennel {kennelId = 1, name = "small", maxLimit = 16, minAnimalSize = 0, maxAminalSize = 20},
                new Kennel {kennelId = 2, name = "medium", maxLimit = 10, minAnimalSize = 20, maxAminalSize = 50},
                new Kennel {kennelId = 3, name = "large", maxLimit = 8, minAnimalSize = 50, maxAminalSize = int.MaxValue}
            };

            var animalData = new List<Animal>()
            {
                new Animal() {animalId = 1, name = "Fido", type = "Dog", sizeInLbs = 12, kennelId = 1, Kennel=kennelData.Where(k=>k.kennelId == 1).First() },
                new Animal() {animalId = 2, name = "Mr. Whiskers", type = "Cat", sizeInLbs = 3, kennelId = 1, Kennel=kennelData.Where(k=>k.kennelId == 1).First() },
                new Animal() {animalId = 3, name = "Spot", type = "Dog", sizeInLbs = 23, kennelId = 2, Kennel=kennelData.Where(k=>k.kennelId == 2).First() },
                new Animal() {animalId = 4, name = "Stretch", type = "Giraffe", sizeInLbs = 1800, kennelId = 3, Kennel=kennelData.Where(k=>k.kennelId == 3).First() },
                new Animal() {animalId = 5, name = "Mr. Ed", type = "Horse", sizeInLbs = 920, kennelId = 3, Kennel=kennelData.Where(k=>k.kennelId == 3).First() },
            };

            context.Kennels.AddRange(kennelData);
            context.Animals.AddRange(animalData);

            context.SaveChanges();
        }
    }
}
