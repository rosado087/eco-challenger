using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class ChallengeController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<GamificationController> _logger;
        

        public ChallengeController(AppDbContext context, ILogger<GamificationController> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new challenge.
        /// </summary>
        /// <param name="challengeModel">The challenge data sent via multipart/form-data.</param>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a list of 
        /// challenges of the user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateChallenge")]
        public async Task<JsonResult> CreateChallenge([FromForm] ChallengeModel challengeModel)
        {
            try{
                var challenge = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Title == challengeModel.Title || c.Description == challengeModel.Description);

                if(challenge != null){
                    return new JsonResult(new{ success = false, message = "Já existe um desafio com esse titulo ou descrição"});
                }

                var newChallenge = new Challenge{
                    Title = challengeModel.Title,
                    Description = challengeModel.Description,
                    Points = challengeModel.Points,
                    Type = challengeModel.Type,
                    MaxProgress = challengeModel.MaxProgress
                };

                await _ctx.Challenges.AddAsync(newChallenge);
                await _ctx.SaveChangesAsync();

                if (challengeModel.UserId != null) {
                var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == challengeModel.UserId);

                if (user != null) {
                    var userChallenge = new UserChallenges {
                        Challenge = newChallenge,
                        User = user,
                        Progress = 0,
                        WasConcluded = false
                    };
                    await _ctx.UserChallenges.AddAsync(userChallenge);
                    await _ctx.SaveChangesAsync();
                }
            }
                
                return new JsonResult(new {success = true, message = "Desafio criado com sucesso"});

            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao adicionar o desafio." });
            }
            
        }

        /// <summary>
        /// Gets all the challenges
        /// </summary>
        /// <returns>
        /// Returns the list of challenges
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllChallenges")]
        public IActionResult GetAllChallenges([FromQuery] string? q)
        {
            try{
                List<Challenge> challenges = [];

                if(string.IsNullOrEmpty(q)) challenges = _ctx.Challenges.ToList();
                else challenges = _ctx.Challenges.Where(t => t.Title != null ? t.Title.Contains(q) : false).ToList();

                if(challenges == null) return Ok(new List<Challenge>());

                return Ok(challenges);

                }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "Ocorreu um erro ao ir buscar os desafios.");
            } 
        }

        /// <summary>
        /// Creates a new challenge.
        /// </summary>
        /// <param name="challengeModel">The challenge data sent via multipart/form-data.</param>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a list of 
        /// challenges of the user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("EditChallenge/{id}")]
        public async Task<JsonResult> EditChallenge([FromForm] ChallengeModel challengeModel, int id)
        {
            try{
                challengeModel.Validate();

                var challenge = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Id == id);

                if(challenge == null){
                    return new JsonResult(new{ success = false, message = "O desafio não existe"});
                }

                var challengeTitle = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Title == challengeModel.Title && c.Id != id);
                if(challengeTitle != null){
                    return new JsonResult(new{ success = false, message = "Já existe um desafio com este título"});
                }

                var challengeDescription = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Description == challengeModel.Description && c.Id != id);
                if(challengeDescription != null){
                    return new JsonResult(new{ success = false, message = "Já existe um desafio com esta descrição"});
                }

                challenge.Title = challengeModel.Title;
                challenge.Description = challengeModel.Description;
                challenge.MaxProgress = challengeModel.MaxProgress;
                challenge.Points = challengeModel.Points;
                challenge.Type = challengeModel.Type;

                await _ctx.SaveChangesAsync();
                
                return new JsonResult(new {success = true, message = "Desafio editado com sucesso"});

            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao editar o desafio." });
            }
            
        }



        /// <summary>
        /// Creates a new challenge.
        /// </summary>
        /// <param name="challengeModel">The challenge data sent via multipart/form-data.</param>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns a list of 
        /// challenges of the user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("DeleteChallenge/{id}")]
        public async Task<JsonResult> DeleteChallenge(int id)
        {
            try{
                var chal = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Id == id);

                if(chal == null){
                    return new JsonResult(new{ success = false, message = "O desafio não existe."});
                }

                _ctx.Challenges.Remove(chal);
                await _ctx.SaveChangesAsync();
                
                return new JsonResult(new {success = true, message = "Desafio removido com sucesso"});

            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao remover o desafio." });
            }
        }

        /// <summary>
        /// Creates a new challenge.
        /// </summary>
        /// <param name="id">The challenges id.</param>
        /// <returns>JSON result indicating success or failure. If failure also returns a message, if success also returns the challenge.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetChallenge/{id}")]
        public async Task<JsonResult> GetChallenge(int id)
        {
            try{
                var chal = await _ctx.Challenges.FirstOrDefaultAsync(c => c.Id == id);

                if(chal == null){
                    return new JsonResult(new{ success = false, message = "O desafio não existe"});
                }
                
                return new JsonResult(new {success = true, message = "Desafio encontrado com sucesso", challenge = chal});

            }catch(Exception e){
                _logger.LogError(e.Message, e.StackTrace);
                return new JsonResult(new { success = false, message = "Ocorreu um erro ao encontrar o desafio" });
            }
        }


    }
}
