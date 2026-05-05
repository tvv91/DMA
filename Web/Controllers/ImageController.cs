using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ImageController(IWebHostEnvironment environment, ILogger<ImageController> logger) : Controller
    {
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png"
        };

        private static readonly Dictionary<string, string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = ".jpg",
            [".jpeg"] = ".jpg",
            [".png"] = ".png"
        };

        private readonly IWebHostEnvironment _environment = environment;
        private readonly ILogger<ImageController> _logger = logger;

        [HttpPost("/uploadimage")]
        [RequestSizeLimit(MaxImageSizeBytes)]
        public async Task<IActionResult> UploadCover([FromForm(Name = "file")] IFormFile? file)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid image upload request.");

            if (file is null)
                return BadRequest("No image file was provided.");

            if (file.Length <= 0 || file.Length > MaxImageSizeBytes)
                return BadRequest("Image file size is invalid.");

            if (!AllowedContentTypes.Contains(file.ContentType))
                return BadRequest("Only JPEG and PNG images are supported.");

            var extension = GetSafeExtension(file.FileName);
            if (extension is null)
                return BadRequest("Only JPEG and PNG images are supported.");

            if (!await HasValidImageSignatureAsync(file, extension))
                return BadRequest("Image file content is invalid.");

            var tempDirectory = GetSafeTempDirectory();
            Directory.CreateDirectory(tempDirectory);

            var filename = $"{Guid.NewGuid():N}{extension}";
            var physicalPath = GetSafeTempFilePath(tempDirectory, filename);

            try
            {
                await using var stream = new FileStream(
                    physicalPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

                await file.CopyToAsync(stream);
                await stream.FlushAsync();

                return Json(new { Filename = filename });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during image upload");
                return BadRequest("Failed to upload image.");
            }
        }

        [HttpDelete("/uploadimage/{filename}")]
        public IActionResult DeleteTempImage(string filename)
        {
            var safeFilename = GetSafeUploadedFilename(filename);
            if (safeFilename is null)
                return BadRequest("Invalid image filename.");

            try
            {
                var tempDirectory = GetSafeTempDirectory();
                var physicalPath = GetSafeTempFilePath(tempDirectory, safeFilename);

                if (System.IO.File.Exists(physicalPath))
                    System.IO.File.Delete(physicalPath);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during temp image deleting {Filename}", filename);
                return BadRequest("Failed to delete temp image.");
            }
        }

        private static string? GetSafeExtension(string fileName)
        {
            var originalName = Path.GetFileName(fileName);
            var extension = Path.GetExtension(originalName);

            return AllowedExtensions.TryGetValue(extension, out var normalizedExtension)
                ? normalizedExtension
                : null;
        }

        private static string? GetSafeUploadedFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            var safeFilename = Path.GetFileName(filename);
            if (!string.Equals(filename, safeFilename, StringComparison.Ordinal))
                return null;

            var extension = GetSafeExtension(safeFilename);
            if (extension is null)
                return null;

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(safeFilename);
            return Guid.TryParseExact(nameWithoutExtension, "N", out _) ? safeFilename : null;
        }

        private static async Task<bool> HasValidImageSignatureAsync(IFormFile file, string extension)
        {
            var buffer = new byte[8];
            await using var stream = file.OpenReadStream();
            var read = await stream.ReadAsync(buffer);

            return extension switch
            {
                ".jpg" => read >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF,
                ".png" => read >= 8 &&
                    buffer[0] == 0x89 &&
                    buffer[1] == 0x50 &&
                    buffer[2] == 0x4E &&
                    buffer[3] == 0x47 &&
                    buffer[4] == 0x0D &&
                    buffer[5] == 0x0A &&
                    buffer[6] == 0x1A &&
                    buffer[7] == 0x0A,
                _ => false
            };
        }

        private string GetSafeTempDirectory()
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            return Path.GetFullPath(Path.Combine(webRoot, "temp"));
        }

        private static string GetSafeTempFilePath(string tempDirectory, string filename)
        {
            var fullTempDirectory = Path.GetFullPath(tempDirectory);
            var fullPath = Path.GetFullPath(Path.Combine(fullTempDirectory, Path.GetFileName(filename)));

            if (!fullPath.StartsWith(fullTempDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Resolved upload path is outside of temp storage.");

            return fullPath;
        }
    }
}
