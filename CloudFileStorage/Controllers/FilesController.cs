using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudFileStorage.Data;
using CloudFileStorage.Models;
using File = CloudFileStorage.Models.File;
using Microsoft.AspNetCore.Authorization;
using CloudFileStorage.Services.Interfaces;

namespace CloudFileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // GET: api/Files/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var response = await _fileService.GetFileByIdAsync(id);
            return Ok(response);
        }

        // GET: api/Files/by_user/5
        [HttpGet("by_user/{userId}")]
        public async Task<IActionResult> GetFilesByUserId(string userId)
        {
            var response = await _fileService.GetFilesByUserIdAsync(userId);
            return Ok(response);
        }

        // GET: api/Files/download/5
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var response = await _fileService.DownloadFileAsync(id);
            return response;
        }

        // POST: api/Files
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("upload")]
        public async Task<IActionResult> PostFile(IFormFile file)
        {
            var uploadedFile = await _fileService.UploadFileAsync(file);
            return Ok(uploadedFile);
        }

        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var response = await _fileService.DeleteFileAsync(id);
            return Ok(new { success = response });
        }
    }
}
