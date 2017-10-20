using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstraction;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/v1/[controller]")]
    public class RevenueController : DogWalkingBaseController
    {
        private IWalkingFacade _wallkingService;

        public RevenueController(IWalkingFacade wallkingService)
        {
            _wallkingService = wallkingService;
        }

        // GET api/revenue
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var revenue = await _wallkingService.CalcRevenue(UserId);

            return Ok(revenue);
        }        
    }
}
