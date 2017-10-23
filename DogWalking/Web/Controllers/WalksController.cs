using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.DataLayer.Abstraction;
using Domain.Models;
using Web.Services.Abstraction;

namespace Web.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    public class WalksController : DogWalkingBaseController
    {
        private IDogWalkersRepository _dogWalkersRepo;
        private IWalkingFacade _walkingService;

        public WalksController(IDogWalkersRepository dogWalkersRepo,
            IWalkingFacade walkingService)
        {
            _dogWalkersRepo = dogWalkersRepo;
            _walkingService = walkingService;
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
            var (statusCode, createdWalks) = await _walkingService.BuildWalksAsync(UserId, payload);

            switch (statusCode)
            {
                case Services.StatusCode.Ok:
                    return Ok(createdWalks);
                case Services.StatusCode.DogWalkerNotFound:
                    return NotFound("Dog walking professional is not found.");
                case Services.StatusCode.SaveDogWalkerDbError:
                    return StatusCode(500, "Couldn't save the result to the DB.");
                default:
                    return StatusCode(500, "Unknown error.");
            }
        }
    }
}
