


using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;

public class DailyTaskService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyTaskService> _logger;
    private Random _random;

    public DailyTaskService(IServiceProvider serviceProvider, ILogger<DailyTaskService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _random = new Random(int.Parse(DateTime.Now.ToString("yyyymmdd")));
        


    }

    /// <summary>
    /// Rotates Daily Challenges of every user every day
    /// </summary>
    /// <param name="stoppingToken">Notifies that an operation has been canceled</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.AddDays(1);
            var delay = nextRun - now;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var dailyChallenges = await dbContext.Challenges.Where(c => c.Type == "Daily").ToListAsync();
                    var UsersDailyChallenges = await dbContext.UserChallenges.Where(dc => dailyChallenges.Contains(dc.Challenge)).ToListAsync();
                    if (UsersDailyChallenges != null)
                    {
                        dbContext.UserChallenges.RemoveRange(UsersDailyChallenges);
                    }
                    
                    var users = dbContext.Users;

                    foreach (var user in users)
                    {
                        List<Challenge> challenges = new List<Challenge>();

                        while (challenges.Count < 3)
                        {
                            var challenge = dailyChallenges[_random.Next(dailyChallenges.Count)];
                            
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

                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation("Rotação de desafios diários foram feitos com sucesso");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer rotação dos desafios diários");
            }

            _logger.LogInformation($"Próxima rotação de desafios diários é {nextRun}");
            await Task.Delay(delay, stoppingToken);
            
        }
    }

    public async void UpdateUserChallenges(User user)
    {
        
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dailyChallenges = await dbContext.Challenges.ToListAsync();

            List<Challenge> challenges = [];

            while (challenges.Count < 3)
            {
                var challenge = dailyChallenges[_random.Next(dailyChallenges.Count)];

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
            await dbContext.SaveChangesAsync();
        }
    }
}