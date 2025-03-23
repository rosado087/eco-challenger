

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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateTag([FromForm] TagCreateModel tagModel)
        {
            try {
                if (string.IsNullOrWhiteSpace(tagModel.Color))
                    return BadRequest("A cor da tag é obrigatória");

                string? imageUrl = null;

                if (tagModel.Icon != null)
                {
                    // Make sure the upload folder exists
                    string uploadPath = Path.Combine(_env.WebRootPath, "tag-images");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

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
                    Color = tagModel.Color,
                    Icon = imageUrl,
                    Style = tagModel.Style,
                    Price = tagModel.Price
                };

                await _ctx.Tags.AddAsync(tag);
                await _ctx.SaveChangesAsync();

                return Ok(tag);
            }
            catch(Exception e) {
                _logger.LogError(e.Message, e.StackTrace);
                return StatusCode(500, "An error occurred creating the tag.");
            }
        }
    }
}
