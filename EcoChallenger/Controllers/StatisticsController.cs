

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
                return StatusCode(500, "An error occurred fethcing the top tags.");
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
                return StatusCode(500, "An error occurred fethcing the top challenges.");
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
                return StatusCode(500, "An error occurred fethcing the top users with most points.");
            }
        }

    }
}
