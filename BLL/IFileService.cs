using DiskSpaceMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.BLL
{
    public interface IFileService
    {
        Task<IEnumerable<FileMetadata>> GetAllFilesAsync();
        Task<FileMetadata> GetFileByIdAsync(int id);
        Task AddFileAsync(FileMetadata file);
        Task UpdateFileAsync(FileMetadata file);
        Task DeleteFileAsync(int id);
    }
}
