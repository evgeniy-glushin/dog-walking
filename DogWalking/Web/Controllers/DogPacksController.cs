using Microsoft.AspNetCore.Mvc;
using Web.DataLayer.Abstraction;
using Web.Services;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DogPacksController : DogWalkingBaseController
    {
        private IDogWalkersRepository _dogWalkersRepo;
        private IWalkingService _wallkingService;

        public DogPacksController(IDogWalkersRepository dogWalkersRepo,
            IWalkingService walkingService)
        {
            _dogWalkersRepo = dogWalkersRepo;
            _wallkingService = walkingService;
        }

        //GET api/dogpacks/get
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dogPacks = await _dogWalkersRepo.GetDogPacks(UserId);

            return Ok(dogPacks);
        }

        //POST api/dogpacks/build
        [HttpPost]
        public async Task<IActionResult> Build()
        {
            var createdPacks = await _wallkingService.BuildDogPacksAsync(UserId);

            return Ok(createdPacks);
        }
    }
}
