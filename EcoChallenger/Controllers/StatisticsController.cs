

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
        /// Creates a new tag with the given data, including an optional image or GIF icon.
        /// </summary>
        /// <param name="tagModel">The tag data sent via multipart/form-data.</param>
        /// <returns>
        /// Returns the created tag
        /// </returns>
        /// <remarks>
        /// The uploaded icon is saved to the wwwroot/tag-images/ folder, and the generated file path is stored.
        /// </remarks>
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
                    .ToList();

                return Ok(new { tags = tagCounts });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fethcing the tags.");
            }
        }

    }
}
