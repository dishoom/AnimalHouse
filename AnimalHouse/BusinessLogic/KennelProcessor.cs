using AnimalHouse.Interface;
using AnimalHouse.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
                var appropriateKennel = _context.Kennels
                    .Where(k => k.maxAminalSize >= animalSizeInLbs)
                    .Where(k => k.minAnimalSize < animalSizeInLbs);

                return await appropriateKennel.FirstOrDefaultAsync();
            }
        }

        public async Task<List<Kennel>> GetKennelsAsync()
        {
            var query = from k in _context.Kennels
                        orderby k.id
                        select k;

            return await query.ToListAsync();
        }

        public async Task<Kennel> GetKennelsByIdAsync(int id)
        {
            var query = from k in _context.Kennels
                        where k.id == id
                        select k;

            return await query.FirstOrDefaultAsync();
        }
    }
}
