using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AnimalHouse.Model;

namespace AnimalHouse.Data
{
    public class AnimalHouseDbContext : DbContext
    {
        public AnimalHouseDbContext() : base("DefaultConnection") { }
        public virtual DbSet<Animal> Animals { get; set; }
        public virtual DbSet<Kennel> Kennels { get; set; }
    }
}
