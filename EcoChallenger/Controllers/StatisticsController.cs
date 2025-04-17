

using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoChallenger.Controllers
{
    public class StatisticsController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<LoginController> _logger;

        private readonly IWebHostEnvironment _env;

        public StatisticsController(AppDbContext context, IWebHostEnvironment env, ILogger<LoginController> logger)
        {
            _env = env;
            _logger = logger;
            _ctx = context;
        }

        /// <summary>
        /// Gets a list with the 10 most purchased tags
        /// </summary>
        /// <returns>
        /// List of objects with tag name and ID, and the purchase count
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("top-purchased-tags")]
        public IActionResult GetTopPurchasedTags()
        {
            try {
                var tagCounts = _ctx.TagUsers
                    .GroupBy(tu => tu.Tag)
                    .Select(g => new {
                        TagName = $"{g.Key.Name} ({g.Key.Id})",
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToList();

                return Ok(new { tags = tagCounts });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching the top tags.");
            }
        }

        /// <summary>
        /// Gets a list with the 10 most completed challenges
        /// </summary>
        /// <returns>
        /// List of objects with challenge name, and the completion count
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("top-completed-challenges")]
        public IActionResult GetTopCompletedChallenges()
        {
            try {
                var challengeCounts = _ctx.UserChallenges
                    .GroupBy(uc => uc.Challenge)
                    .Select(g => new {
                        ChallengeName = g.Key.Title,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToList();

                return Ok(new { challenges = challengeCounts });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching the top challenges.");
            }
        }

        /// <summary>
        /// Gets a list with the 5 users with the most points
        /// </summary>
        /// <returns>
        /// List of objects with username, and point count
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("top-users-most-points")]
        public IActionResult GetTopUsersMostPoints()
        {
            try {
                var userPointsCount = _ctx.Users
                    .Select(u => new {
                        u.Username,
                        u.Points
                    })
                    .OrderByDescending(x => x.Points)
                    .Take(5)
                    .ToList();

                return Ok(new { users = userPointsCount });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching the top users with most points.");
            }
        }

        /// <summary>
        /// Gets a list with the 5 friends with the most points
        /// </summary>
        /// <returns>
        /// List of objects with username, and point count
        /// </returns>
        [HttpGet]
        [Route("top-friends-most-points")]
        public IActionResult GetTopFriendsMostPoints()
        {
            try {
                var topFriendsByPoints = _ctx.Friendships
                    .Where(f => f.UserId == UserContext.Id)
                    .Select(f => f.FriendId)
                    .Join(_ctx.Users,
                        friendId => friendId,
                        user => user.Id,
                        (friendId, user) => new {
                            user.Username,
                            user.Points
                        })
                    .OrderByDescending(u => u.Points)
                    .Take(5)
                    .ToList();


                return Ok(new { users = topFriendsByPoints });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching the top users with most points.");
            }
        }

        /// <summary>
        /// Gets the amount of users that logged-in in a given month
        /// </summary>
        /// <returns>
        /// List of objects with username, and point count
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("monthly-active-users")]
        public IActionResult GetMonthlyActiveUsers([FromQuery] int? year, [FromQuery] int? month)
        {
            try {
                if(year == null) year = DateTime.Now.Year;
                if(month == null) month = DateTime.Now.Month;

                var monthlyLogins = _ctx.Users
                    .Where(u => u.LastLogin.Year == year && u.LastLogin.Month == month)
                    .GroupBy(u => u.LastLogin.Date)
                    .Select(g => new {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Date)

                    // We need to this whole thing because an exception will
                    // be thrown using ToShortDateString above
                    // aparently SQL tries to interpret that, it has to do with the way
                    // entity framework works
                    .AsEnumerable()
                    .Select(g => new {
                        Date = g.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        g.Count
                    })
                    .ToList();
                 
                return Ok(new { logins = monthlyLogins });
            }
            catch(Exception e) {
                _logger.LogError(e, "Exception while fetching monthly active users");
                return StatusCode(500, "An error occurred fetching the monthly active users.");
            }
        }
    }
}
