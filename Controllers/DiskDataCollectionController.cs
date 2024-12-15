using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DiskSpaceMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiskDataCollectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiskDataCollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("collect")]
        public async Task<IActionResult> CollectDiskInfo([FromBody] DiskDataRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ServerName) || request.DiskInfoList == null)
            {
                return BadRequest("Invalid request. ServerName and DiskInfoList are required.");
            }

            try
            {
                var service = new DiskDataService(_context);
                var result = await service.SaveDiskData(request.ServerName, request.DiskInfoList);

                if (!result)
                {
                    return NotFound($"Server '{request.ServerName}' not found.");
                }

                return Ok("Data collected and saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // DTO для запроса
        public class DiskDataRequest
        {
            public string ServerName { get; set; } // Имя сервера, переданное в запросе
            public List<DiskInfoDto> DiskInfoList { get; set; } // Список дисков
        }

        // DTO для дисков
        public class DiskInfoDto
        {
            public string Name { get; set; } // Название диска, например "C:"
            public long TotalSize { get; set; } // Общий размер диска
            public long FreeSpace { get; set; } // Свободное место на диске
        }
    }
}
