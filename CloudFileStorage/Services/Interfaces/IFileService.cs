using CloudFileStorage.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using File = CloudFileStorage.Models.File;

namespace CloudFileStorage.Services.Interfaces
{
    public interface IFileService
    {
        Task<FileResponse> UploadFileAsync(IFormFile file);
        Task<IActionResult> DownloadFileAsync(Guid id);
        Task<FileResponse> GetFileByIdAsync(Guid id);
        Task<List<FileResponse>> GetFilesByUserIdAsync(string userId);

        Task<List<FileResponse>> GetAllFilesAsync();
        Task<bool> DeleteFileAsync(Guid id);
    }
}
