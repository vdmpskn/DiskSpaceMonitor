using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiskSpaceMonitor.Controllers { 
[Route("api/[controller]")]
[ApiController]
public class QuotasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuotasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Quotas
    [HttpPost]
    public async Task<IActionResult> PostQuota([FromBody] QuotaDto quotaDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var server = await _context.Servers.FirstOrDefaultAsync(s => s.Name == quotaDto.ServerName);
        if (server == null)
        {
            return NotFound($"Server with name {quotaDto.ServerName} not found.");
        }

        var quota = new Quota
        {
            Path = quotaDto.Path,
            TotalSize = quotaDto.TotalSize,
            FreeSize = quotaDto.FreeSize,
            UsageSize = quotaDto.UsageSize,
            ServerId = server.Id,
            GrowthTime = quotaDto.GrowthTime ?? DateTime.Now // Установка текущего времени, если не предоставлено
        };

        _context.Quotas.Add(quota);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetQuota", new { id = quota.Id }, quota);
    }
}
}
