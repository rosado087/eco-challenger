using EcoChallenger.Models;
using EcoChallenger.Services;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace EcoChallenger.Controllers
{
    public class RegisterController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private Random _random;

        public RegisterController(AppDbContext context)
        {
            _ctx = context;
            _random = new Random(int.Parse(DateTime.Now.ToString("yyyymmdd")));
        }

        /// <summary>
        /// Handles the action that creates a new User.
        /// Verifies if the email which the User wrote is not being used already.
        /// If not, creates a new hashed password.
        /// Add the new User into the database.
        /// </summary>
        /// <param name="data">Contains the user's username, email address and password</param>
        /// <returns>JSON result indicating success or failure. If returns failure also returns the error message</returns>
        [AllowAnonymous]
        [HttpPost("RegisterAccount")]
        public async Task<JsonResult> RegisterAccount([FromBody] User data) {
            try
            {
                // Verifica se o email já existe
                var emailExists = await _ctx.Users.AnyAsync(x => x.Email == data.Email);

                if (emailExists)
                    return new JsonResult(new { success = false, message = "Este email já existe" });

                if(data.Password != null)
                    data.Password = PasswordGenerator.GeneratePasswordHash(data.Password);

                // Adiciona um novo utilizador
                await _ctx.Users.AddAsync(data);

                //Secção de atribuição de desafios
                var dailyChallenges = await _ctx.Challenges.Where(c => c.Type == "Daily").ToListAsync();

                List<Challenge> challenges = [];

                while (challenges.Count < 3)
                {
                    var challenge = dailyChallenges[_random.Next(dailyChallenges.Count)];

                    if (challenge != null && !challenges.Contains(challenge))
                    {
                        await _ctx.UserChallenges.AddAsync(new UserChallenges
                        {
                            Challenge = challenge,
                            User = data,
                            WasConcluded = false
                        });

                        challenges.Add(challenge);
                    }
                }

                var weeklyChallenges = await _ctx.Challenges.Where(c => c.Type == "Weekly").ToListAsync();

                challenges = [];

                while (challenges.Count < 2)
                {
                    var challenge = weeklyChallenges[_random.Next(weeklyChallenges.Count)];

                    if (challenge != null && !challenges.Contains(challenge))
                    {
                        await _ctx.UserChallenges.AddAsync(new UserChallenges
                        {
                            Challenge = challenge,
                            User = data,
                            Progress = 0,
                            WasConcluded = false
                        });
                        challenges.Add(challenge);
                    }
                }

                await _ctx.SaveChangesAsync();
                
                

                return new JsonResult(new { success = true });
            }
            catch (Exception e)
            {
                // Retorna uma resposta JSON com a mensagem de erro
                return new JsonResult(new { success = false, message = e.Message });
            }                
        }

        /*private async void AddChallenges(User user)
        {
            var dailyChallenges = await _ctx.Challenges.ToListAsync();

            List<Challenge> challenges = [];

            while (challenges.Count < 3)
            {
                var challenge = dailyChallenges[_random.Next(dailyChallenges.Count)];

                if (challenge != null && !challenges.Contains(challenge))
                {
                    await _ctx.UserChallenges.AddAsync(new UserChallenges
                    {
                        Challenge = challenge,
                        User = user,
                        WasConcluded = false
                    });

                    challenges.Add(challenge);
                }
            }

            var weeklyChallenges = await _ctx.Challenges.Where(c => c.Type == "Weekly").ToListAsync();

            challenges = [];

            while (challenges.Count < 2)
            {
                var challenge = weeklyChallenges[_random.Next(weeklyChallenges.Count)];

                if (challenge != null && !challenges.Contains(challenge))
                {
                    await _ctx.UserChallenges.AddAsync(new UserChallenges
                    {
                        Challenge = challenge,
                        User = user,
                        Progress = 0,
                        WasConcluded = false
                    });
                    challenges.Add(challenge);
                }
            }
            await _ctx.SaveChangesAsync();
        }*/
    }
}