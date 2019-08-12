using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHouse.Model
{
    public class Kennel
    {
        internal int count;

        public int id { get; set; }
        public string name { get; set; }
        public int minAnimalSize { get; set; }
        public int maxAminalSize { get; set; }
        public int maxLimit { get; set; }
    }
}
