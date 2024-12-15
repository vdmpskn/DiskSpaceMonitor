using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class DiskController : Controller
{
    private readonly ApplicationDbContext _context;

    public DiskController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Метод для відображення списку дисків
    public async Task<IActionResult> Index(string serverGroup = "All", int? serverId = null)
    {
        try
        {
            // Получаем список дисков с включением серверов
            IQueryable<Disk> disks = _context.Disks.Include(d => d.Server);

            // Фильтрация по серверу или группе серверов
            if (serverId.HasValue)
            {
                disks = disks.Where(d => d.ServerId == serverId);
            }

            // Исключаем диск C:
            disks = disks.Where(d => d.Name != "C:");

            var diskList = await disks.ToListAsync();

            // Данные для графиков: используем данные по всей системе
            ViewBag.TotalUsedSpace = diskList.Sum(d => d.TotalSpace - d.FreeSpace);
            ViewBag.TotalFreeSpace = diskList.Sum(d => d.FreeSpace);

            // Данные для фильтрации по серверу
            ViewBag.Servers = new SelectList(await _context.Servers.ToListAsync(), "Id", "Name");

            // Данные для графиков передаем как есть, для всех серверов
            ViewBag.UsedSpaceData = diskList.Select(d => d.TotalSpace - d.FreeSpace).ToArray(); // Используемое пространство
            ViewBag.FreeSpaceData = diskList.Select(d => d.FreeSpace).ToArray(); // Свободное пространство
            ViewBag.DiskLabels = diskList.Select(d => d.Name).ToArray();

            return View(diskList);
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Error in DiskController.Index: {ex.Message}");

            // Показываем ошибку на странице
            return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }


}
