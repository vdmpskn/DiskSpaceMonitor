using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using DiskSpaceMonitor.Controllers;

public class DiskDataService
{
    private readonly ApplicationDbContext _context;

    public DiskDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveDiskData(string serverName, List<DiskDataCollectionController.DiskInfoDto> diskInfoList)
    {
        var server = await _context.Servers.FirstOrDefaultAsync(s => s.Name == serverName);
        if (server == null)
        {
            return false; // Сервер не найден
        }

        foreach (var diskInfo in diskInfoList)
        {
            if (diskInfo.TotalSize <= 0) continue; // Пропуск некорректных данных

            var existingDisk = await _context.Disks
                .FirstOrDefaultAsync(d => d.Name == diskInfo.Name && d.ServerId == server.Id);

            if (existingDisk == null)
            {
                _context.Disks.Add(new Disk
                {
                    Name = diskInfo.Name,
                    TotalSpace = diskInfo.TotalSize,
                    FreeSpace = diskInfo.FreeSpace,
                    ServerId = server.Id
                });
            }
            else
            {
                existingDisk.TotalSpace = diskInfo.TotalSize;
                existingDisk.FreeSpace = diskInfo.FreeSpace;
                _context.Disks.Update(existingDisk);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
