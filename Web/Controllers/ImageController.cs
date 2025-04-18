using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Web.Controllers
{
    public class ImageController : Controller
    {
        [HttpPost("/uploadimage")]
        public async Task<IActionResult> UploadCover()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var files = HttpContext.Request.Form.Files;
                if (files.Any())
                {
                    var guid = Guid.NewGuid().ToString("N");
                    var ext = Path.GetExtension(files[0].FileName);
                    await using var target = new MemoryStream();
                    await files[0].CopyToAsync(target);
                    var physicalPath = $"{new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp")).Root}{$@"{guid}{ext}"}";
                    await using FileStream fs = System.IO.File.Create(physicalPath);
                    await files[0].CopyToAsync(fs);
                    fs.Flush();
                    return Json(new { Filename = $"{guid}{ext}" });
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
