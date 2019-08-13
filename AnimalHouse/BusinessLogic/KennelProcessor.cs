using AnimalHouse.Data;
using AnimalHouse.Interface;
using AnimalHouse.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalHouse.BusinessLogic
{
    public class KennelProcessor : IKennelProcessor
    {
        private AnimalHouseDbContext _context;

        public KennelProcessor(AnimalHouseDbContext context)
        {
            _context = context;
        }

        public async Task<Kennel> GetKennelByAnimalSizeAsync(double animalSizeInLbs)
        {
            using (_context)
            {
                return await _context.Kennels
                    .Where(k => k.maxAminalSize >= animalSizeInLbs)
                    .Where(k => k.minAnimalSize < animalSizeInLbs)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<List<Kennel>> GetKennelsAsync()
        {
            using (_context)
            {
                var kennels = await _context.Kennels
                    .OrderBy(k => k.kennelId)
                    .ToListAsync();

                return kennels;
            }
        }

        public async Task<Kennel> GetKennelsByIdAsync(int id)
        {
            using (_context)
            {
                var kennel = await _context.Kennels
                    .Where(k => k.kennelId == id)
                    .FirstOrDefaultAsync();

                return kennel;
            }
        }

        public async Task<List<KennelAnimals>> GetAnimalsInEachKennelAsync()
        {
            using (_context)
            {                
                var kennelAnimalReport = new List<KennelAnimals>();

                var kennels = await _context.Kennels
                                        .OrderBy(k => k.kennelId)
                                        .ToListAsync();

                foreach (var kennel in kennels)
                {
                    var kennelAnimals = new KennelAnimals{
                        kennelId = kennel.kennelId,
                        name = kennel.name,
                        minAnimalSize = kennel.minAnimalSize,
                        maxAminalSize = kennel.maxAminalSize,
                        maxLimit = kennel.maxLimit
                    };

                    kennelAnimals.animals = await _context.Animals
                                                    .Where(a => a.kennelId == kennel.kennelId)
                                                    .ToListAsync();

                    kennelAnimalReport.Add(kennelAnimals);
                }

                return kennelAnimalReport;
            }
        }
    }
}
