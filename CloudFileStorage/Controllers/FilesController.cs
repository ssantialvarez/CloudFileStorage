using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudFileStorage.Data;
using CloudFileStorage.Helpers;
using CloudFileStorage.Models;
using File = CloudFileStorage.Models.File;
using Microsoft.AspNetCore.Authorization;
using CloudFileStorage.Services.Interfaces;

namespace CloudFileStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // GET: api/Files
        /// <summary>
        /// Gets all files uploaded. Admin only.
        /// </summary>
        /// <returns>Successfully retrieved files.</returns>
        /// <response code="200">Returns all files</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[Produces("application/json")]
        public async Task<IActionResult> GetFiles()
        {
            var response = await _fileService.GetAllFilesAsync();
            return Ok(response);
        }

        // GET: api/Files/5
        /// <summary>
        /// Gets specific file by its id. Admin only.
        /// </summary>
        /// <returns>Successfully retrieved file.</returns>
        /// <response code="200">Returns the file</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[Produces("application/json")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var response = await _fileService.GetFileByIdAsync(id);
            return Ok(response);
        }

        // GET: api/Files/by_user/5
        /// <summary>
        /// Gets files uploaded by certain user. Admin only.
        /// </summary>
        /// <returns>Successfully retrieved files.</returns>
        /// <response code="200">Returns all files of the user.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("by_user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFilesByUserId(string userId)
        {
            var response = await _fileService.GetFilesByUserIdAsync(userId);
            return Ok(response);
        }

        // GET: api/Files/stats
        /// <summary>
        /// Gets stats. Admin only.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return stats.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        /// <response code="403">Forbidden. Admin Only.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStats()
        {
            var response = await _fileService.GetStatsAsync();
            return Ok(response);       
        }

        // GET: api/Files/me
        /// <summary>
        /// Gets all the files you have uploaded. 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return stats.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOwnFiles()
        {
            
            var response = await _fileService.GetOwnFilesAsync();
            return Ok(response);
        }

        // GET: api/Files/download/5
        /// <summary>
        /// Downloads file by id. 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return stats.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpGet("download/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var response = await _fileService.DownloadFileAsync(id);
            return response;
        }

        // POST: api/Files
        /// <summary>
        /// Uploads file. 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return stats.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostFile(IFormFile file)
        {
            var uploadedFile = await _fileService.UploadFileAsync(file);
            return Ok(uploadedFile);
        }

        // DELETE: api/Files/5
        /// <summary>
        /// Deletes file. 
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return stats.</response>
        /// <response code="401">Unauthorized. Login or register.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var response = await _fileService.DeleteFileAsync(id);
            return Ok(new { success = response });
        }
    }
}
