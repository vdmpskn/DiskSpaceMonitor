using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Главная страница
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

                // Передаем данные в ViewBag для отображения на главной странице
                ViewBag.TotalSpace = diskList.Sum(d => d.TotalSpace); // Общий объем всех дисков
                ViewBag.TotalUsedSpace = diskList.Sum(d => d.TotalSpace - d.FreeSpace); // Используемое пространство
                ViewBag.TotalFreeSpace = diskList.Sum(d => d.FreeSpace); // Свободное пространство

                // Передаем данные для графиков
                ViewBag.UsedSpaceData = diskList.Select(d => d.TotalSpace - d.FreeSpace).ToArray(); // Используемое пространство
                ViewBag.FreeSpaceData = diskList.Select(d => d.FreeSpace).ToArray(); // Свободное пространство
                ViewBag.DiskLabels = diskList.Select(d => d.Name).ToArray(); // Метки для графика

                // Данные для фильтрации по серверу (если нужно)
                ViewBag.Servers = new SelectList(await _context.Servers.ToListAsync(), "Id", "Name");

                return View(diskList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in HomeController.Index: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
