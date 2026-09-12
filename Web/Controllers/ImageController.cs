using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Features.Supporting;

namespace Web.Controllers
{
    public class ImageController(ISender sender) : Controller
    {
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private readonly ISender _sender = sender;

        [HttpPost("/uploadimage")]
        [RequestSizeLimit(MaxImageSizeBytes)]
        public async Task<IActionResult> UploadCover([FromForm(Name = "file")] IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid image upload request.");

            var result = await _sender.Send(new ImageUploadCommand(file));
            return result.Success ? Json(new { result.Filename }) : BadRequest(result.Error);
        }

        [HttpDelete("/uploadimage/{filename}")]
        public async Task<IActionResult> DeleteTempImage(string filename)
        {
            return await _sender.Send(new DeleteTempImageCommand(filename))
                ? Ok()
                : BadRequest("Invalid image filename.");
        }
    }
}
