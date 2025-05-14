using Microsoft.AspNetCore.Mvc;

namespace CloudFileStorage.Helpers;

public interface IProviderHandler
{
    Task<bool> UploadFileAsync(IFormFile file, string? prefix);
    Task<string> DownloadFileAsync(string fileName, string? prefix);
    Task<bool> DeleteFileAsync(string fileName, string? prefix);
}