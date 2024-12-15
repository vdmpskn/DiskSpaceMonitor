using DiskSpaceMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.DAL
{
    public interface IFileRepository
    {
        Task<IEnumerable<FileMetadata>> GetAllFilesAsync();
        Task<FileMetadata> GetFileByIdAsync(int id);
        Task AddFileAsync(FileMetadata file);
        Task UpdateFileAsync(FileMetadata file);
        Task DeleteFileAsync(int id);
    }
}
