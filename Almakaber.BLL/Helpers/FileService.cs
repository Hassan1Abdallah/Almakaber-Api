using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Almakaber.BLL.Helpers
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return string.Empty;

            string webRootPath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                ? Path.Combine(_env.ContentRootPath, "wwwroot")
                : _env.WebRootPath;

            string uploadsFolder = Path.Combine(webRootPath, "images", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{folderName}/{uniqueFileName}";
        }

        public void DeleteFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var filePath = Path.Combine(_env.WebRootPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}