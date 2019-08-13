using AnimalHouse.BusinessLogic;
using AnimalHouse.Data;
using AnimalHouse.Model;
using AnimalHouseAPI.Models;
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

        public AnimalController()
        {
            var context = new AnimalHouseDbContext();
            var kennelProcessor = new KennelProcessor(context);
            _animalProcessor = new AnimalProcessor(context, kennelProcessor);
        }
        
        [Route("api/Animal/Add")]
        [HttpPost]
        public async Task<HttpResponseMessage> Add([FromBody]AnimalModel animal)
        {
            double lbs;

            if (animal == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal info is required.");
            if (string.IsNullOrEmpty(animal.name))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Name is required.");
            if (string.IsNullOrEmpty(animal.type))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal type is required.");
            if (!double.TryParse(animal.sizeInLbs.ToString(), out lbs))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Size is not valid.");

            try
            {
                var success = await _animalProcessor.AddAnimalToShelterAsync(new Animal { name = animal.name, type = animal.type, sizeInLbs = animal.sizeInLbs });

                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal was not added to shelter.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error adding animal to shelter.", ex);
            }
            
        }

        [Route("api/Animal/RemoveById")]
        [HttpPost]
        public async Task<HttpResponseMessage> RemoveById([FromBody] int animalId)
        {
            if (!int.TryParse(animalId.ToString(), out animalId))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal Id is not valid");

            try
            {
                var success = await _animalProcessor.RemoveAnimalById(animalId);

                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal was not removed.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error removing animal from shelter.", ex);
            }
        }

        [Route("api/Animal/RemoveAnimal")]
        [HttpPost]
        public async Task<HttpResponseMessage> RemoveAnimal([FromBody]AnimalModel animal)
        {
            double lbs;

            if (animal == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal info is required.");
            if (string.IsNullOrEmpty(animal.name))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Name is required.");
            if (string.IsNullOrEmpty(animal.type))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal type is required.");
            if (!double.TryParse(animal.sizeInLbs.ToString(), out lbs))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Size is not valid.");

            try
            {
                var success = await _animalProcessor.RemoveAnimalByNameAndTypeAndSize(animal.name, animal.type, animal.sizeInLbs);
                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Animal was not removed.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error removing animal from shelter.", ex);
            }            
        }

        [Route("api/Animal/ReorganizeAnimals")]
        [HttpGet]
        public async Task<HttpResponseMessage> ReorganizeAnimalsAsync()
        {
            try
            {
                var success = await _animalProcessor.ReorganizeAnimalsToAppropriateKennels();

                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not reorganize, animal(s) will be left behind."); //consideration: create custom exception for this specific outcome and relay to back to user
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Error reorganizing animals.", ex);
            }
        }
    }
}
