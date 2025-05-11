using File = CloudFileStorage.Models.File;

namespace CloudFileStorage.Services.Interfaces


{
    public interface IFileService
    {
        Task<File> UploadFileAsync(IFormFile file);
        Task<File> GetFileByIdAsync(Guid id);
        Task<bool> DeleteFileAsync(Guid id);
    }
}
