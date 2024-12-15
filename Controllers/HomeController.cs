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

        // ������� ��������
        public async Task<IActionResult> Index(string serverGroup = "All", int? serverId = null)
        {
            try
            {
                // �������� ������ ������ � ���������� ��������
                IQueryable<Disk> disks = _context.Disks.Include(d => d.Server);

                // ���������� �� ������� ��� ������ ��������
                if (serverId.HasValue)
                {
                    disks = disks.Where(d => d.ServerId == serverId);
                }

                // ��������� ���� C:
                disks = disks.Where(d => d.Name != "C:");

                var diskList = await disks.ToListAsync();

                // �������� ������ � ViewBag ��� ����������� �� ������� ��������
                ViewBag.TotalSpace = diskList.Sum(d => d.TotalSpace); // ����� ����� ���� ������
                ViewBag.TotalUsedSpace = diskList.Sum(d => d.TotalSpace - d.FreeSpace); // ������������ ������������
                ViewBag.TotalFreeSpace = diskList.Sum(d => d.FreeSpace); // ��������� ������������

                // �������� ������ ��� ��������
                ViewBag.UsedSpaceData = diskList.Select(d => d.TotalSpace - d.FreeSpace).ToArray(); // ������������ ������������
                ViewBag.FreeSpaceData = diskList.Select(d => d.FreeSpace).ToArray(); // ��������� ������������
                ViewBag.DiskLabels = diskList.Select(d => d.Name).ToArray(); // ����� ��� �������

                // ������ ��� ���������� �� ������� (���� �����)
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
