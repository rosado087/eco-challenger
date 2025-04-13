

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoChallenger.Controllers
{
    public class TagController : BaseApiController
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<LoginController> _logger;

        private readonly IWebHostEnvironment _env;

        public TagController(AppDbContext context, IWebHostEnvironment env, ILogger<LoginController> logger)
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
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateTag([FromForm] TagCRUDModel tagModel)
        {
            try {
                if (_ctx.Tags.Any(t => t.Name == tagModel.Name))
                    return BadRequest(new { success = false, message = "Esta tag já existe." });

                string? imageUrl = null;

                if (tagModel.Icon != null)
                {
                    string uploadPath = GetUploadPath();

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(tagModel.Icon.FileName)}";
                    string filePath = Path.Combine(uploadPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await tagModel.Icon.CopyToAsync(stream);

                    imageUrl = $"/tag-images/{fileName}";
                }

                // Insert new tag in the DB
                Tag tag = new Tag
                {
                    Name = tagModel.Name,
                    BackgroundColor = tagModel.BackgroundColor,
                    TextColor = tagModel.TextColor,
                    Icon = imageUrl,
                    Style = tagModel.Style,
                    Price = tagModel.Price
                };

                await _ctx.Tags.AddAsync(tag);
                await _ctx.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred creating the tag.");
            }
        }

        private string GetUploadPath() {
            // Make sure the upload folder exists
            string uploadPath = Path.Combine(_env.WebRootPath, "tag-images");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            return uploadPath;
        }

        /// <summary>
        /// Edits a tag with a given ID
        /// </summary>
        /// <param name="id">The ID of the tag to edit.</param>
        /// <param name="tagModel">The tag data sent via multipart/form-data.</param>
        /// <returns>
        /// Success true or false
        /// </returns>
        /// <remarks>
        /// The uploaded icon is saved to the wwwroot/tag-images/ folder, and the generated file path is stored.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("edit/{id}")]
        public async Task<IActionResult> EditTag(int id, [FromForm] TagCRUDModel tagModel)
        {
            try {
                var tag = _ctx.Tags
                    .Where(t => t.Id == id)
                    .FirstOrDefault();

                if(tag == null) return StatusCode(404, "There is no tag with the given ID.");

                // If the user uploaded an icon, we will just replace the previous one
                // there is no way to comapre if they are equal
                string? imageUrl = null;
                if (tagModel.Icon != null)
                {
                    string uploadPath = GetUploadPath();

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(tagModel.Icon.FileName)}";
                    string filePath = Path.Combine(uploadPath, fileName);

                    imageUrl = $"/tag-images/{fileName}";

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await tagModel.Icon.CopyToAsync(stream);

                    // Remove old icon
                    if(!string.IsNullOrEmpty(tag.Icon))
                        RemoveImage(tag.Icon);
                }

                // Update the DB record
                tag.Name = tagModel.Name;
                tag.BackgroundColor = tagModel.BackgroundColor;
                tag.TextColor = tagModel.TextColor;
                if(tagModel.Icon != null) tag.Icon = imageUrl;
                tag.Style = tagModel.Style;
                tag.Price = tagModel.Price;

                _ctx.Tags.Update(tag);
                _ctx.SaveChanges();

                return Ok(new { success = true });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred editing the tag.");
            }
        }

        /// <summary>
        /// Gets all the tags available
        /// </summary>
        /// <returns>
        /// Returns the list of tags
        /// </returns>
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllTags([FromQuery] string? q)
        {
            try {
                List<Tag> tags = [];
                if(string.IsNullOrEmpty(q)) tags = _ctx.Tags.ToList();
                else tags = _ctx.Tags.Where(t => t.Name != null ? t.Name.Contains(q) : false).ToList();

                if(tags == null) return Ok(new List<Tag>());

                return Ok(tags);
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching system tags.");
            }
        }

        /// <summary>
        /// Gets all the tags for a given user ID
        /// </summary>
        /// <returns>
        /// Returns the list of tags
        /// </returns>
        [HttpGet]
        [Route("user/{id}")]
        public IActionResult GetAllTags(int id)
        {
            try {
                List<UserTag> tags = _ctx.Tags
                    .Where(t => _ctx.TagUsers.Any(tu => tu.User.Id == id && tu.Tag.Id == t.Id))
                    .Select(t => new UserTag
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Price = t.Price,
                        BackgroundColor = t.BackgroundColor,
                        TextColor = t.TextColor,
                        Icon = t.Icon,
                        Style = t.Style,
                        IsBeingUsed = _ctx.TagUsers.Any(tu => tu.User.Id == id && tu.Tag.Id == t.Id && tu.SelectedTag)
                    })
                    
                    .ToList();

                return Ok(tags);
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching system tags.");
            }
        }

        /// <summary>
        /// Gets all the tags available
        /// </summary>
        /// <returns>
        /// Returns the list of tags
        /// </returns>
        [HttpGet]
        [Route("per-id/{id}")]
        public IActionResult GetTag(int id)
        {
            try {
                var tag = _ctx.Tags.Where(t => t.Id == id).FirstOrDefault();

                if(tag == null) return StatusCode(404, "A tag with the given ID was not found");

                return Ok(tag);
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching system tags.");
            }
        }

        private void RemoveImage(string path) {
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }

        /// <summary>
        /// Gets all the tags available
        /// </summary>
        /// <returns>
        /// Returns the list of tags
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("remove/{id}")]
        public IActionResult RemoveTag(int id)
        {
            try {
                var tag = _ctx.Tags
                    .Where(t => t.Id == id)
                    .FirstOrDefault();

                if(tag == null) return StatusCode(404, "There is no tag with the given ID.");

                string path = "";
                if(!string.IsNullOrEmpty(tag.Icon))
                    path = Path.Combine(_env.WebRootPath, tag.Icon);

                _ctx.Tags.Remove(tag);
                _ctx.SaveChanges();

                // Make sure there is no image leftover
                RemoveImage(path);
                
                return Ok(new { success = true });
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred fetching system tags.");
            }
        }
    }
}
