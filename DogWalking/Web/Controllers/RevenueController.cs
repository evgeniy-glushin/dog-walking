using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstraction;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class RevenueController : DogWalkingBaseController
    {
        private IWalkingFacade _walkingService;

        public RevenueController(IWalkingFacade walkingService)
        {
            _walkingService = walkingService;
        }

        // GET api/revenue
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var revenue = await _walkingService.CalcRevenue(UserId);

            return Ok(revenue);
        }        
    }
}
