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
        public async Task<ActionResult<File>> GetFile(Guid id)
        {
            throw new NotImplementedException();
        }

        // POST: api/Files
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("upload")]
        public async Task<ActionResult<File>> PostFile()
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
