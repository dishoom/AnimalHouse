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
    public class KennelController : ApiController
    {
        private KennelProcessor _kennelProcessor;

        public KennelController(KennelProcessor kennelProcessor)
        {
            _kennelProcessor = kennelProcessor;
        }

        // GET: api/Kennel
        public async Task<IEnumerable<Kennel>> Get()
        {
            return await _kennelProcessor.GetKennelsAsync();
        }

        // GET: api/Kennel/5
        public async Task<Kennel> Get(int id)
        {
            int kennelId;

            if (!int.TryParse(id.ToString(), out kennelId))
                throw new ArgumentException("Kennel Id", "Kennel Id is not valid");

            return await _kennelProcessor.GetKennelsByIdAsync(id);
        }

        public async Task<List<KennelAnimals>> GetKennelReport()
        {
            return await _kennelProcessor.GetAnimalsInEachKennelAsync();
        }
    }
}
