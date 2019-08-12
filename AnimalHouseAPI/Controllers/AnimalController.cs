using AnimalHouse.BusinessLogic;
using AnimalHouse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AnimalHouseAPI.Controllers
{
    public class AnimalController : ApiController
    {
        private AnimalProcessor _animalProcessor;

        public AnimalController(AnimalProcessor animalProcessor)
        {
            _animalProcessor = animalProcessor;
        }

        public async Task<Animal> Get(int id)
        {
            return await _animalProcessor.GetAnimalById(id);
        }

        public async Task PostAsync([FromBody]Animal value)
        {
            double lbs;

            if (string.IsNullOrEmpty(value.name))
                throw new ArgumentNullException("Name", "Name is required.");
            if (string.IsNullOrEmpty(value.type))
                throw new ArgumentNullException("Animal Type", "Animal type is required.");
            if (!double.TryParse(value.sizeInLbs.ToString(), out lbs))
                throw new ArgumentException("Size", "Size is not valid");

            await _animalProcessor.AddAnimalToShelterAsync(value);
        }
        
        public async Task Delete(int id)
        {
            int animalId;

            if (!int.TryParse(id.ToString(), out animalId))
                throw new ArgumentException("Animal Id", "Animal Id is not valid");

            await _animalProcessor.RemoveAnimalById(id);
        }

        public async Task DeleteByNameTypeSize(string name, string type, double sizeInLbs)
        {
            double lbs;

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name", "Name is required.");
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("Animal Type", "Animal type is required.");
            if (!double.TryParse(sizeInLbs.ToString(), out lbs))
                throw new ArgumentException("Size", "Size is not valid");

            await _animalProcessor.RemoveAnimalByNameAndTypeAndSize(name, type, sizeInLbs);
        }

        public async Task ReorganizeAnimalsAsync()
        {
            await _animalProcessor.ReorganizeAnimalsToAppropriateKennels();
        }
    }
}
