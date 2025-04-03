using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoChallenger.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly IWebHostEnvironment _environment;

        public TestController(IWebHostEnvironment env) {
            _environment = env;

            if(!_environment.IsEnvironment("Test"))
                throw new Exception("Application must be running in test mode");
        }

#if DEBUG
        /// <summary>
        /// Method just used in automation testing to retrieve recovery tokens
        /// </summary>
        /// <returns>List of recovery tokens</returns>
        [AllowAnonymous]
        [HttpGet("get-recovery-tokens")]
        public IActionResult getRecovertTokens() {                       
            return Ok(FakeEmailService.Tokens);
        }      
    }
#endif

}
