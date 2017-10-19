using Microsoft.AspNetCore.Mvc;
using Web.DataLayer.Abstraction;
using Web.Services;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    public class DogPacksController : DogWalkingBaseController
    {
        private IDogWalkersRepository _dogWalkersRepo;
        private IWalkingFacade _wallkingService;

        public DogPacksController(IDogWalkersRepository dogWalkersRepo,
            IWalkingFacade walkingService)
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
            var (statusCode, createdPacks) = await _wallkingService.BuildDogPacksAsync(UserId);

            switch (statusCode)
            {
                case Services.StatusCode.Ok:
                    return Ok(createdPacks);
                case Services.StatusCode.DogWalkerNotFound:
                    return NotFound("Dog walking professional not found.");
                case Services.StatusCode.SaveDogWalkerDbError:
                    return StatusCode(500, "Couldn't save the result to the DB.");
                default:
                    return StatusCode(500, "Unknown error.");
            }            
        }
    }
}
