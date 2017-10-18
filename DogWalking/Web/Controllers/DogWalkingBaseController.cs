using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class DogWalkingBaseController : Controller
    {
        protected int UserId => 1; // Should be taken from JWT token in real world
    }
}
