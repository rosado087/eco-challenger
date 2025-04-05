


using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;

public class DailyTaskService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DailyTaskService> _logger;
    private Random _random;

    public DailyTaskService(IServiceScopeFactory serviceScopeFactory, ILogger<DailyTaskService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
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
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await UpdateUserChallenges(null, true, dbContext);
                    await dbContext.SaveChangesAsync();
                }
               _logger.LogInformation("Rotação de desafios diários foram feitos com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer rotação dos desafios diários");
            }

            _logger.LogInformation($"Próxima rotação de desafios diários é {nextRun}");
            await Task.Delay(delay, stoppingToken);
            
        }
    }

    public async Task UpdateUserChallenges(User? userReceived, bool isRotation, AppDbContext dbContext)
    {
        User[] users = [userReceived];

        //using (var scope = _serviceScopeFactory.CreateScope())
        //{
            //var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dailyChallenges = await dbContext.Challenges.Where(c => c.Type == "Daily").ToListAsync();
            

            if (isRotation)
            {
                var UsersDailyChallenges = await dbContext.UserChallenges.Where(dc => dc.Challenge.Type == "Daily").ToListAsync();
                if (UsersDailyChallenges != null)
                {
                    dbContext.UserChallenges.RemoveRange(UsersDailyChallenges);
                }
                users = await dbContext.Users.ToArrayAsync();
            }
            
            List<Challenge> challenges = [];
            foreach(var user in users) {
                challenges.Clear();
                while (challenges.Count < 3 && dailyChallenges.Count >= 2)
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
        //}

        
    }
}