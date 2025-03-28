using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoChallenger.Services
{
    public class WeeklyTaskService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyTaskService> _logger;
        private Random _random;

        public WeeklyTaskService(IServiceProvider serviceProvider, ILogger<DailyTaskService> logger)
        {
            _serviceProvider = serviceProvider;
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

                _logger.LogInformation($"Próxima rotação de desafios semanais é {nextRun}");

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var weeklyChallenges = await dbContext.Challenges.Where(c => c.Type == "Weekly").ToListAsync();
                        var UsersWeeklyChallenges = await dbContext.UserChallenges.Where(dc => weeklyChallenges.Contains(dc.Challenge)).ToListAsync();
                        if (UsersWeeklyChallenges != null)
                        {
                            dbContext.UserChallenges.RemoveRange(UsersWeeklyChallenges);
                        }

                        var users = dbContext.Users;

                        foreach (var user in users)
                        {
                            List<Challenge> challenges = [];

                            while (challenges.Count < 2)
                            {
                                var challenge = weeklyChallenges[_random.Next(weeklyChallenges.Count)];

                                if (challenge != null && !challenges.Contains(challenge))
                                {
                                    await dbContext.UserChallenges.AddAsync(new UserChallenges
                                    {
                                        Challenge = challenge,
                                        User = user,
                                        Progress = 0,
                                        WasConcluded = false
                                    });
                                    challenges.Add(challenge);
                                }
                            }
                        }
                        await dbContext.SaveChangesAsync();

                        _logger.LogInformation("Rotação de desafios semanais foram feitos com sucesso");

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao fazer rotação dos desafios semanais");
                }
            }
        }

        public async void UpdateUserChallenges(User user)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var weeklyChallenges = await dbContext.Challenges.Where(c => c.Type == "Weekly").ToListAsync();

                List<Challenge> challenges = [];

                while (challenges.Count < 2)
                {
                    var challenge = weeklyChallenges[_random.Next(weeklyChallenges.Count)];

                    if (challenge != null && !challenges.Contains(challenge))
                    {
                        await dbContext.UserChallenges.AddAsync(new UserChallenges
                        {
                            Challenge = challenge,
                            User = user,
                            Progress = 0,
                            WasConcluded = false
                        });
                        challenges.Add(challenge);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
