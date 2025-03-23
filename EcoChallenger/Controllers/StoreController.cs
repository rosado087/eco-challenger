

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
                    tag.Price,
                    Style = tag.Style.ToString().ToLower(),
                    Owned = ownedTagIds.Any(t => t.Id == tag.Id)
                });

                return Ok(result);
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred loading the store tags");
            }            
        }

        /// <summary>
        /// Gets all the tags in the system, with information if they are owned by the current
        /// user or not
        /// </summary>
        /// <returns>
        /// Returns the list of tags
        /// </returns>
        [HttpPost("purchase/{id}")]
        public IActionResult PurchaseTag(int id)
        {
            try {
                // Make sure the user doesn't own the tag yet
                var ownedTag = _ctx.TagUsers
                    .Where(ut => ut.User.Id == UserContext.Id)
                    .Where(ut => ut.Tag.Id == id)
                    .FirstOrDefault();

                if(ownedTag != null)
                    return Ok(new {success = false, message = "O utilizadore já comprou esta tag."});

                var tag = _ctx.Tags
                    .Where(t => t.Id == id)
                    .FirstOrDefault();

                if(tag == null)
                    throw new InvalidOperationException("The purchased tag does not exist, or has invalid ID.");
                
                var user = _ctx.Users
                    .Where(u => u.Id == UserContext.Id)
                    .FirstOrDefault();

                if(user == null)
                    throw new InvalidOperationException("Could not find the corresponding user record.");

                // Make sure the user has enough points
                if(user.Points < tag.Price)
                    return Ok(new { success = false, message = "O utilizadore não tem pontos suficientes!"});

                // Attribute the tag to the user
                TagUsers tu = new TagUsers {
                    SelectedTag = false,
                    Tag = tag,
                    User = user
                };
                _ctx.TagUsers.Add(tu);

                // Deduct points from user
                user.Points -= tag.Price;
                _ctx.Users.Update(user);
                _ctx.SaveChanges();
                
                return Ok(new { success = true });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred purchasing the tag.");
            }            
        }
    }
}
