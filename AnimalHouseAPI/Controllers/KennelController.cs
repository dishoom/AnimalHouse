using AnimalHouse.BusinessLogic;
using AnimalHouse.Data;
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

        public KennelController()
        {
            _kennelProcessor = new KennelProcessor(new AnimalHouseDbContext());
        }

        public KennelController(KennelProcessor kennelProcessor)
        {
            _kennelProcessor = kennelProcessor;
        }
                
        [Route("api/KennelReport")]
        [HttpGet]
        public async Task<List<KennelAnimals>> GetKennelReport()
        {
            return await _kennelProcessor.GetAnimalsInEachKennelAsync();
        }
    }
}
