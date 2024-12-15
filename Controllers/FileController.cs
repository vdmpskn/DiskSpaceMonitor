using DiskSpaceMonitor.BLL;
using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context; // Додано для отримання серверів і дисків з бази даних

        public FileController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context; // Ініціалізація контексту бази даних
        }

        public async Task<IActionResult> Index()
        {
            var files = await _fileService.GetAllFilesAsync();
            return View(files);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            // Передаємо списки серверів і дисків у ViewBag
            ViewBag.Servers = new SelectList(await _context.Servers.ToListAsync(), "Id", "Name");
            ViewBag.Disks = new SelectList(await _context.Disks.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileMetadata file)
        {
            if (ModelState.IsValid)
            {
                await _fileService.AddFileAsync(file);
                return RedirectToAction(nameof(Index));
            }

            // Якщо модель невалідна, повертаємо списки серверів і дисків
            ViewBag.Servers = new SelectList(await _context.Servers.ToListAsync(), "Id", "Name", file.ServerId);
            ViewBag.Disks = new SelectList(await _context.Disks.ToListAsync(), "Id", "Name", file.DiskId);
            return View(file);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileMetadata = await _fileService.GetFileByIdAsync(id.Value);
            if (fileMetadata == null)
            {
                return NotFound();
            }

            // Передаємо списки серверів і дисків у ViewBag для випадаючих списків
            ViewBag.Servers = new SelectList(await _context.Servers.ToListAsync(), "Id", "Name", fileMetadata.ServerId);
            ViewBag.Disks = new SelectList(await _context.Disks.ToListAsync(), "Id", "Name", fileMetadata.DiskId);
            return View(fileMetadata);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FileMetadata file)
        {
            if (id != file.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _fileService.UpdateFileAsync(file);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await FileExists(file.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Якщо модель невалідна, повертаємо списки серверів і дисків
            ViewBag.Servers = new SelectList(await _context.Servers.ToListAsync(), "Id", "Name", file.ServerId);
            ViewBag.Disks = new SelectList(await _context.Disks.ToListAsync(), "Id", "Name", file.DiskId);
            return View(file);
        }

        // Перевірка наявності файлу
        private async Task<bool> FileExists(int id)
        {
            return await _fileService.GetFileByIdAsync(id) != null;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileMetadata = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fileMetadata == null)
            {
                return NotFound();
            }

            return View(fileMetadata);  // Показуємо підтвердження видалення
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fileMetadata = await _context.Files.FindAsync(id);

            if (fileMetadata != null)
            {
                _context.Files.Remove(fileMetadata);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));  // Повертаємося до списку після видалення
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileMetadata = await _context.Files
                .Include(f => f.Server)  // Завантажуємо пов'язаний сервер
                .Include(f => f.Disk)    // Завантажуємо пов'язаний диск
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fileMetadata == null)
            {
                return NotFound();
            }

            return View(fileMetadata);
        }



    }
}
