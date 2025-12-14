using Microsoft.AspNetCore.Mvc;

namespace ApiProject.ApiLogic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public EnvironmentController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult GetEnvironment()
        {
            return Ok(new { environment = _env.EnvironmentName });
        }
    }
}
