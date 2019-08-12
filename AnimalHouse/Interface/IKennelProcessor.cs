using AnimalHouse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHouse.Interface
{
    public interface IKennelProcessor
    {
        Task<Kennel> GetKennelByAnimalSizeAsync(double animalSizeInLbs);
    }
}
