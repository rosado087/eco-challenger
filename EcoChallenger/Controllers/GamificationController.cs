using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
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

        /// <summary>
        /// Completes a challenge.
        /// </summary>
        /// <param name="id">Id of the completed challenge.</param>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns </returns>
        [AllowAnonymous]
        [HttpPost("CompleteChallenge/{id}")]
        public async Task<JsonResult> CompleteChallenge(int id)
        {
            var userChallenge = await _ctx.UserChallenges.
            FirstOrDefaultAsync(x => x.Challenge.Id == id && x.User.Id == 1);

            if (userChallenge == null)
                return new JsonResult(new {success = false, message = "O desafio não existe." });

            var challenge = await _ctx.UserChallenges
            .Where(x => x.Challenge.Id == userChallenge.Challenge.Id && x.User.Id == 1)
            .Select(x => x.Challenge).FirstOrDefaultAsync();

            if (challenge == null)
                return new JsonResult(new {success = false, message = "O desafio não existe." });

            userChallenge.User.Points += challenge.Points;
            userChallenge.WasConcluded = true;
            await _ctx.SaveChangesAsync();

            var weeklyChallenges = await _ctx.UserChallenges
            .Where(x => x.User.Id == UserContext.Id && x.Challenge.Type == "Weekly")
            .Select(x => new{challenge = x.Challenge, progress = x.Progress, wasConcluded = x.WasConcluded })
            .ToListAsync();
            
            var dailyChallenges = await _ctx.UserChallenges
            .Where(x => x.User.Id == UserContext.Id && x.Challenge.Type == "Daily")
            .Select(x => new { challenge = x.Challenge, wasConcluded = x.WasConcluded })
            .ToListAsync();

            return new JsonResult(new { 
                success = true, 
                message = "Desafio concluido com sucesso.",
                dailyChallenges = dailyChallenges,
                weeklyChallenges = weeklyChallenges, 
            });
        }
    }
}
