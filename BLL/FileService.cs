using DiskSpaceMonitor.DAL;
using DiskSpaceMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiskSpaceMonitor.BLL
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<IEnumerable<FileMetadata>> GetAllFilesAsync()
        {
            return await _fileRepository.GetAllFilesAsync();
        }

        public async Task<FileMetadata> GetFileByIdAsync(int id)
        {
            return await _fileRepository.GetFileByIdAsync(id);
        }

        public async Task AddFileAsync(FileMetadata file)
        {
            await _fileRepository.AddFileAsync(file);
        }

        public async Task UpdateFileAsync(FileMetadata file)
        {
            await _fileRepository.UpdateFileAsync(file);
        }

        public async Task DeleteFileAsync(int id)
        {
            await _fileRepository.DeleteFileAsync(id);
        }
    }
}
