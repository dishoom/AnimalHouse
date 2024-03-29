﻿using AnimalHouse.Data;
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
    public class AnimalProcessor : IAnimalProcessor
    {
        private AnimalHouseDbContext _context;
        private KennelProcessor _kennelProcessor;

        public AnimalProcessor(AnimalHouseDbContext context, KennelProcessor kennelProcessor)
        {
            _context = context;
            _kennelProcessor = kennelProcessor;
        }

        public async Task<bool> AddAnimalToShelterAsync(Animal animal)
        {

            //find appropriate kennel based on animal size
            //compare kennel capacity with current kennel count
            //add animal to kennel if kennel count is below capacity

            using (_context)
            {
                var animalAccepted = false;

                var appropriateKennel = await _context.Kennels
                    .Where(k => k.maxAminalSize >= animal.sizeInLbs)
                    .Where(k => k.minAnimalSize < animal.sizeInLbs)
                    .FirstOrDefaultAsync();

                var numberOfAnimalsInKennel = await _context.Animals
                    .Where(a => a.kennelId == appropriateKennel.kennelId)
                    .CountAsync();


                if (numberOfAnimalsInKennel < appropriateKennel.maxLimit)
                {
                    animal.kennelId = appropriateKennel.kennelId;
                    _context.Animals.Add(animal);
                    var whatisthisint = await _context.SaveChangesAsync();
                    animalAccepted = true;
                }

                return animalAccepted;
            }
        }

        public async Task<List<Animal>> GetAnimalsInKennelAsync(int kennelId)
        {
            using (_context)
            {
                var animals = await _context.Animals
                    .Where(a => a.kennelId == kennelId)
                    .ToListAsync();

                return animals;
            }
        }

        public async Task<Animal> GetAnimalByIdAsync(int animalId)
        {
            using (_context)
            {
                var animal = await _context.Animals
                    .Where(a => a.animalId == animalId)
                    .FirstOrDefaultAsync();

                return animal;
            }
        }

        public async Task<Animal> GetAnimalByNameAndTypeAndSizeAsync(string name, string type, double size)
        {
            using (_context)
            {
                var animal = await _context.Animals
                    .Where(a => a.name == name)
                    .Where(a => a.type == type)
                    .Where(a => a.sizeInLbs == size)
                    .FirstOrDefaultAsync();

                return animal;
            }
        }

        public async Task<bool> RemoveAnimalByIdAsync(int animalId)
        {
            using (_context)
            {
                var animalToRemove = _context.Animals.Where(a => a.animalId == animalId).FirstOrDefault();
                
                if (animalToRemove != null)
                {
                    _context.Animals.Remove(animalToRemove);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false; //or throw not found exception
                }
            }
        }

        public async Task<bool> RemoveAnimalByNameAndTypeAndSizeAsync(string name, string type, double size)
        {
            using (_context)
            {
                var animalToRemove = await _context.Animals
                    .Where(a => a.name == name)
                    .Where(a => a.type == type)
                    .Where(a => a.sizeInLbs == size)
                    .FirstOrDefaultAsync();

                if (animalToRemove != null)
                {
                    _context.Animals.Remove(animalToRemove);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false; //or throw not found exception
                }
            }
        }

        public async Task<bool> ReorganizeAnimalsToAppropriateKennelsAsync()
        {
            //Get list of kennels and their properties
            //For each animal, reorganize into approriate kennel (without commiting changes to db)
            //Perform check: if reorganiztion results in over capacity of any kennel
            //               or if animal does not fit in any kennel
            //               then do not commit changes to db (no animal left behind!) and send error message to user
            //if all is good, then update animal's kennel (commit changes to db)
            using (_context)
            {
                var success = true;

                var kennels = await _context.Kennels
                    .OrderBy(k => k.kennelId)
                    .ToListAsync();

                kennels.ForEach(k => k.count = 0); //reset count for each kennel


                var animals = await _context.Animals
                    .ToListAsync();

                animals.ForEach(a => a.kennelId = 0);

                foreach (var animal in animals)
                {
                    var kennelForThisAnimal = await _context.Kennels
                                                    .Where(k => k.maxAminalSize >= animal.sizeInLbs)
                                                    .Where(k => k.minAnimalSize < animal.sizeInLbs)
                                                    .FirstOrDefaultAsync();

                    if (kennelForThisAnimal != null)
                        animal.kennelId = kennelForThisAnimal.kennelId;
                    else
                        success = false; //condition met if no kennels accept this animal's size
                }

                foreach (var kennel in kennels)
                {
                    var animalCountInKennel = animals.Where(a => a.kennelId == kennel.kennelId).Count();

                    if (animalCountInKennel > kennel.maxLimit)
                        success = false; //condition met if kennels are over capacity                     
                }

                //only commit changes if no animals are left behind after the restructure
                if (success)                
                    await _context.SaveChangesAsync();     
                
                return success;  
            }
        }
    }
}
