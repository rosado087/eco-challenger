

using Microsoft.AspNetCore.Mvc;

namespace EcoChallenger.Controllers
{
    public class StoreController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<LoginController> _logger;

        private readonly IWebHostEnvironment _env;

        public StoreController(AppDbContext context, IWebHostEnvironment env, ILogger<LoginController> logger)
        {
            _env = env;
            _logger = logger;
            _ctx = context;
        }

        /// <summary>
        /// TODO: Make this only accessible to Admin users
        /// Creates a new tag with the given data, including an optional image or GIF icon.
        /// </summary>
        /// <param name="tagModel">The tag data sent via multipart/form-data.</param>
        /// <returns>
        /// Returns the created tag
        /// </returns>
        /// <remarks>
        /// The uploaded icon is saved to the wwwroot/tag-images/ folder, and the generated file path is stored.
        /// </remarks>
        [HttpPost]
        [HttpGet("tags")]
        public async Task<IActionResult> GetAvailableTags()
        {
            var userId = GetCurrentUserId(); // however you extract the user from the context

            var allTags = await _ctx.Tags.ToListAsync();
            var ownedTagIds = await _ctx.UserTags
                .Where(ut => ut.UserId == userId)
                .Select(ut => ut.TagId)
                .ToListAsync();

            var result = allTags.Select(tag => new
            {
                tag.Id,
                tag.Name,
                tag.Color,
                tag.Icon,
                tag.Style,
                Owned = ownedTagIds.Contains(tag.Id)
            });

            return Ok(result);
        }
    }
}
