using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimalHouseAPI.Models
{
    public class AnimalModel
    {
        public string name { get; set; }
        public string type { get; set; }
        public double sizeInLbs { get; set; }
    }
}