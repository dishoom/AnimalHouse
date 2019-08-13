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
        Task<bool> RemoveAnimalByNameAndTypeAndSizeAsync(string name, string type, double size);
        Task<bool> RemoveAnimalByIdAsync(int animalId);
        Task<bool> ReorganizeAnimalsToAppropriateKennelsAsync();
    }
}
