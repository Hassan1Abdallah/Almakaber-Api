using Microsoft.AspNetCore.Http;

namespace Almakaber.BLL.Helpers
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);

        void DeleteFile(string fileName, string folderName);
    }
}