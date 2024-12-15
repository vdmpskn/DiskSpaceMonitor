using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.Controllers
{
    public class FolderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FolderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> FolderUsage(string filterBy = null, string filterValue = null, string timePeriod = "180")
        {
            IQueryable<Folder> folders = _context.Folders.Include(f => f.Server);

            // Фильтрация по времени
            DateTime currentDate = DateTime.Now;
            DateTime startDate;

            switch (timePeriod)
            {
                case "7":
                    startDate = currentDate.AddDays(-7);
                    break;
                case "30":
                    startDate = currentDate.AddDays(-30);
                    break;
                case "180":
                default:
                    startDate = currentDate.AddDays(-180);
                    break;
            }

            folders = folders.Where(f => f.GrowthDate >= startDate); // Фильтрация по дате

            // Фильтрация по другим параметрам (Company, Service, Department, Sector)
            if (!string.IsNullOrEmpty(filterValue))
            {
                switch (filterBy)
                {
                    case "Company":
                        folders = folders.Where(f => f.Company == filterValue);
                        break;
                    case "Service":
                        folders = folders.Where(f => f.Service == filterValue);
                        break;
                    case "Department":
                        folders = folders.Where(f => f.Department == filterValue);
                        break;
                    case "Sector":
                        folders = folders.Where(f => f.Sector == filterValue);
                        break;
                }
            }

            // Получаем список всех папок, соответствующих фильтрам
            var folderList = await folders.ToListAsync();

            // Подготовка данных для фильтрации
            ViewBag.Companies = folderList.Select(f => f.Company).Distinct().ToList();
            ViewBag.Services = folderList.Select(f => f.Service).Distinct().ToList();
            ViewBag.Departments = folderList.Select(f => f.Department).Distinct().ToList();
            ViewBag.Sectors = folderList.Select(f => f.Sector).Distinct().ToList();

            // Для отображения изменений по времени (суммирование по дням)
            var growthData = folderList
                .GroupBy(f => f.GrowthDate.Date)  // Группируем по дате
                .Select(g => new
                {
                    GrowthDate = g.Key,
                    TotalSize = g.Sum(f => f.Size)  // Суммируем размеры по каждой дате
                })
                .OrderBy(g => g.GrowthDate)
                .ToList();

            // Подготовка данных для графиков изменения
            ViewBag.GrowthDates = growthData.Select(g => g.GrowthDate.ToString("yyyy-MM-dd")).ToArray();  // Форматируем дату
            ViewBag.TotalSizes = growthData.Select(g => g.TotalSize).ToArray();  // Передаем размеры для графика

            // Передаем данные для визуализации (размеры по каждому фильтру)
            ViewBag.CompanyLabels = folderList.Select(f => f.Company).Distinct().ToArray();
            ViewBag.ServiceLabels = folderList.Select(f => f.Service).Distinct().ToArray();
            ViewBag.DepartmentLabels = folderList.Select(f => f.Department).Distinct().ToArray();
            ViewBag.SectorLabels = folderList.Select(f => f.Sector).Distinct().ToArray();

            // Размеры в гигабайтах по каждой категории
            ViewBag.CompanySizes = folderList
                .GroupBy(f => f.Company)
                .Select(g => g.Sum(f => f.Size))
                .ToArray();

            ViewBag.ServiceSizes = folderList
                .GroupBy(f => f.Service)
                .Select(g => g.Sum(f => f.Size))
                .ToArray();

            ViewBag.DepartmentSizes = folderList
                .GroupBy(f => f.Department)
                .Select(g => g.Sum(f => f.Size))
                .ToArray();

            ViewBag.SectorSizes = folderList
                .GroupBy(f => f.Sector)
                .Select(g => g.Sum(f => f.Size))
                .ToArray();

            // Периоды времени для фильтрации
            ViewBag.TimePeriod = timePeriod; // Передаем выбранный временной период в представление

            return View(folderList);
        }
    }
}
