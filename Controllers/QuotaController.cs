using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class QuotaController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuotaController(ApplicationDbContext context)
    {
        _context = context;
    }

    private (string Company, string Service) ParsePath(string path)
    {
        var parts = path.Split('\\');
        if (parts.Length > 3)
            return (parts[2], parts[3]);
        if (parts.Length > 2)
            return (parts[2], "N/A"); // Если Service отсутствует
        return (null, null);
    }

    public async Task<IActionResult> Index(string company = null, string service = null)
    {
        // Извлекаем все записи из базы данных
        var quotas = await _context.Quotas
            .Include(q => q.Server)
            .ToListAsync();

        if (!quotas.Any())
        {
            ViewBag.ErrorMessage = "No quotas available.";
            ViewBag.CompanyList = new SelectList(Enumerable.Empty<string>());
            ViewBag.ServiceList = new SelectList(Enumerable.Empty<string>());
            ViewBag.ChartData = new List<object>();
            return View(new List<Quota>());
        }

        // Применяем `ParsePath` для извлечения компании и отдела
        var parsedQuotas = quotas
            .Select(q => new
            {
                Quota = q,
                Company = ParsePath(q.Path).Company,
                Service = ParsePath(q.Path).Service
            })
            .Where(q => (string.IsNullOrEmpty(company) || q.Company == company) &&
                        (string.IsNullOrEmpty(service) || q.Service == service))
            .ToList();

        // Заполняем списки компаний и служб
        var companies = quotas
            .Select(q => ParsePath(q.Path).Company)
            .Distinct()
            .Where(c => !string.IsNullOrEmpty(c))
            .ToList();

        var services = quotas
            .Where(q => string.IsNullOrEmpty(company) || ParsePath(q.Path).Company == company)
            .Select(q => ParsePath(q.Path).Service)
            .Distinct()
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        ViewBag.CompanyList = new SelectList(companies);
        ViewBag.ServiceList = new SelectList(services);

        // Подготовка данных для графика
        var chartData = parsedQuotas
            .GroupBy(q => q.Company)
            .Select(g => new
            {
                Company = g.Key,
                FreeSize = g.Sum(x => x.Quota.FreeSize),
                UsageSize = g.Sum(x => x.Quota.UsageSize)
            })
            .ToList();

        ViewBag.ChartData = chartData;

        // Передаем данные в представление
        return View(parsedQuotas.Select(q => q.Quota).ToList());
    }

}
