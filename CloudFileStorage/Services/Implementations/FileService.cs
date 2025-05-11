using CloudFileStorage.Services.Interfaces;
using File = CloudFileStorage.Models.File;

namespace CloudFileStorage.Services.Implementations
{
    public class FileService : IFileService
    {
        public Task<bool> DeleteFileAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<File> GetFileByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<File> UploadFileAsync(IFormFile file)
        {
            // Implementation for uploading a file
            throw new NotImplementedException();
        }
    }
}
