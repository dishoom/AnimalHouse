using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHouse.Model
{
    public class Animal
    {
        [Key]
        public int animalId { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public double sizeInLbs { get; set; }
        public int kennelId { get; set; }
        public Kennel Kennel { get; set; }
    }
}
