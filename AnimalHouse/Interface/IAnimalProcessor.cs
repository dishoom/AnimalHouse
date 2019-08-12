using AnimalHouse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHouse.Interface
{
    public interface IAnimalProcessor
    {
        Task<bool> AddAnimalToShelterAsync(Animal animal);
        //Task<bool> RemoveAnimalByName(string animalName);
        //Task<bool> RemoveAnimalById(int animalId);
        //Task<bool> ReorganizeAnimalsToAppropriateKennels();
    }
}
