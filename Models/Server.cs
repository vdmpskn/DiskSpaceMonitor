namespace DiskSpaceMonitor.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; } // Назва сервера
        public string IPAddress { get; set; } // IP-адреса сервера
        public DateTime LastChecked { get; set; } // Дата останньої перевірки сервера
        public bool IsActive { get; set; } // Чи активний сервер

        // Колекція файлів, пов'язаних із сервером
        public ICollection<FileMetadata> Files { get; set; }

        // Колекція дисків, пов'язаних із сервером
        public ICollection<Disk> Disks { get; set; } // Додано для зв'язку "один сервер — багато дисків"
    }
}
