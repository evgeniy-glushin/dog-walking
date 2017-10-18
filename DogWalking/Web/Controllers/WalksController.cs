using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Threading.Tasks;
using Web.DataLayer.Abstraction;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class WalksController : DogWalkingBaseController
    {
        private IDogWalkersRepository _dogWalkersRepo;
        private IWalkingService _wallkingService;

        public WalksController(IDogWalkersRepository dogWalkersRepo,
            IWalkingService walkingService)
        {
            _dogWalkersRepo = dogWalkersRepo;
            _wallkingService = walkingService;
        }

        //GET api/walks/get
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var walks = await _dogWalkersRepo.GetWalks(UserId);

            return Ok(walks);
        }

        //POST api/walks/build
        [HttpPost]
        public async Task<IActionResult> Build([FromBody] BuildWalksPayload payload)
        {
            var createdWalks = await _wallkingService.BuildWalksAsync(UserId, payload);

            return Ok(createdWalks);
        }
    }
}
