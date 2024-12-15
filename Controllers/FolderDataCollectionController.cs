using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiskSpaceMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FolderDataCollectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FolderDataCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("collect")]
        public async Task<IActionResult> CollectFolderData([FromBody] FolderDataRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ServerName) || request.FolderInfoList == null)
            {
                return BadRequest("Invalid request. ServerName and FolderInfoList are required.");
            }

            try
            {
                var server = await _context.Servers.FirstOrDefaultAsync(s => s.Name == request.ServerName);
                if (server == null)
                {
                    return NotFound($"Server '{request.ServerName}' not found.");
                }

                foreach (var folderInfo in request.FolderInfoList)
                {
                    var existingFolder = await _context.Folders
                        .FirstOrDefaultAsync(f =>
                            f.Company == folderInfo.Company &&
                            f.Service == folderInfo.Service &&
                            f.Department == folderInfo.Department &&
                            f.Sector == folderInfo.Sector &&
                            f.ServerId == server.Id);

                    if (existingFolder == null)
                    {
                        _context.Folders.Add(new Folder
                        {
                            Company = folderInfo.Company,
                            Service = folderInfo.Service,
                            Department = folderInfo.Department,
                            Sector = folderInfo.Sector,
                            Size = folderInfo.Size,
                            ServerId = server.Id,
                       
                        });
                    }
                    else
                    {
                        existingFolder.Size = folderInfo.Size;
                        _context.Folders.Update(existingFolder);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Folder data collected and saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
