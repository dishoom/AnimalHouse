using AnimalHouse.BusinessLogic;
using AnimalHouse.Data;
using AnimalHouse.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                
        [Route("api/Kennel/Report")]
        [HttpGet]
        public async Task<HttpResponseMessage> Report()
        {
            try
            {
                var kennelReport = await _kennelProcessor.GetAnimalsInEachKennelAsync();
                return Request.CreateResponse(HttpStatusCode.OK, kennelReport);
            }
            catch (Exception ex)
            {
                //Consideration: Add logger i.e. logger.error(ex)
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error generating report.", ex);
            }            
        }
    }
}
