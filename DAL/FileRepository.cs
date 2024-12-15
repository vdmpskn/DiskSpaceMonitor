using DiskSpaceMonitor.Data;
using DiskSpaceMonitor.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.DAL
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;

        public FileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FileMetadata>> GetAllFilesAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<FileMetadata> GetFileByIdAsync(int id)
        {
            return await _context.Files.FindAsync(id);
        }

        public async Task AddFileAsync(FileMetadata file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFileAsync(FileMetadata file)
        {
            _context.Files.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }
    }
}
