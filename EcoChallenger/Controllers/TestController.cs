using Microsoft.AspNetCore.Mvc;

namespace EcoChallenger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("testme", Name = "testme")]
        public JsonResult TestMe()
        {
            return new JsonResult(new { success = true });
        }
    }
}
