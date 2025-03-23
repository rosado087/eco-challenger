

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class StoreController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<LoginController> _logger;

        public StoreController(AppDbContext context, ILogger<LoginController> logger)
        {
            _logger = logger;
            _ctx = context;
        }

        /// <summary>
        /// Gets all the tags in the system, with information if they are owned by the current
        /// user or not
        /// </summary>
        /// <returns>
        /// Returns the list of tags
        /// </returns>
        [HttpGet("tags")]
        public async Task<IActionResult> GetStoreTags()
        {
            try {
                var allTags = await _ctx.Tags.ToListAsync();
                var ownedTagIds = await _ctx.TagUsers
                    .Where(ut => ut.User.Id == UserContext.Id)
                    .Select(ut => ut.Tag)
                    .ToListAsync();

                var result = allTags.Select(tag => new
                {
                    tag.Id,
                    tag.Name,
                    tag.Color,
                    tag.Icon,
                    tag.Style,
                    Owned = ownedTagIds.Any(t => t.Id == tag.Id)
                });

                return Ok(result);
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred loading the store tags");
            }            
        }
    }
}
