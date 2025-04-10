using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoChallenger.Services
{
    public class WeeklyTaskService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<WeeklyTaskService> _logger;
        private Random _random;

        public WeeklyTaskService(IServiceScopeFactory serviceScopeFactory, ILogger<WeeklyTaskService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _random = new Random(int.Parse(DateTime.Now.ToString("yyyymmdd"))); 
        }

        /// <summary>
        /// Rotates Weekly Challenges of every user every 7 days
        /// </summary>
        /// <param name="stoppingToken">Notifies that an operation has been canceled</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = now.Date.AddDays(7);
                var delay = nextRun - now;

                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await UpdateUserChallenges(null, true, dbContext);
                        await dbContext.SaveChangesAsync();

                    }
                    _logger.LogInformation("Rotação de desafios semanais foram feitos com sucesso");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao fazer rotação dos desafios semanais");
                }

                _logger.LogInformation($"Próxima rotação de desafios semanais é {nextRun}");
                await Task.Delay(delay, stoppingToken);

            }
        }

        public async Task UpdateUserChallenges(User? userReceived, bool isRotation, AppDbContext dbContext)
        {
            User[] users = [userReceived];

            //(using (var scope = _serviceScopeFactory.CreateScope())
            //{
                //var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var weeklyChallenges = await dbContext.Challenges.Where(c => c.Type == "Weekly").ToListAsync();


                if (isRotation)
                {
                    var UsersWeeklyChallenges = await dbContext.UserChallenges.Where(dc => dc.Challenge.Type == "Weekly").ToListAsync();
                    if (UsersWeeklyChallenges != null)
                    {
                        dbContext.UserChallenges.RemoveRange(UsersWeeklyChallenges);
                    }
                    users = await dbContext.Users.ToArrayAsync();
                }
                
                List<Challenge> challenges = [];
                foreach (var user in users)
                {
                    challenges.Clear();
                    while (challenges.Count < 2 && weeklyChallenges.Count >= 2)
                    {
                        var challenge = weeklyChallenges[_random.Next(weeklyChallenges.Count)];

                        if (challenge != null && !challenges.Contains(challenge))
                        {
                            await dbContext.UserChallenges.AddAsync(new UserChallenges
                            {
                                Challenge = challenge,
                                User = user,
                                WasConcluded = false
                            });


                            challenges.Add(challenge);
                        }
                    }
                }
                
            //}

        }
    }
}
