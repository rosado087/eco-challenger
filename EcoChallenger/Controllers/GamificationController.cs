using EcoChallenger.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class GamificationController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<ProfileController> _logger;

        public GamificationController(AppDbContext context, ILogger<ProfileController> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns challenges of the user.
        /// </summary>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a list of 
        /// challenges of the user</returns>

        [HttpGet("GetChallenges")]
        public async Task<JsonResult> GetChallenges()
        {
            var weeklyChallenges = await _ctx.UserChallenges.Where(x => x.User.Id == UserContext.Id && x.Challenge.Type == "Weekly").Select(x => new{challenge = x.Challenge, progress = x.Progress, wasConcluded = x.WasConcluded }).ToListAsync();
            var dailyChallenges = await _ctx.UserChallenges.Where(x => x.User.Id == UserContext.Id && x.Challenge.Type == "Daily").Select(x => new { challenge = x.Challenge, wasConcluded = x.WasConcluded }).ToListAsync();
            if (weeklyChallenges == null || dailyChallenges == null)
                return new JsonResult(new { success = false, message = "Ocorreu um problema ao encontrar  os desafios atribuidos." });

            return new JsonResult(new
            {
                success = true,
                message = "Os desafios do utilizador foram encontrados com sucesso.",
                dailyChallenges = dailyChallenges,
                weeklyChallenges = weeklyChallenges,
            });
        }
    }
}
