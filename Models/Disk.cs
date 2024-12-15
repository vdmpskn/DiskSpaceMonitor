namespace DiskSpaceMonitor.Models
{
    public class Disk
    {
        public int Id { get; set; }
        public string Name { get; set; } // Наприклад, "C:"
        public long TotalSpace { get; set; } // Загальний простір на диску
        public long FreeSpace { get; set; } // Вільний простір на диску
        public int ServerId { get; set; } // Зв'язок із сервером
        public Server Server { get; set; } // Відношення до сервера

        // Колекція файлів на цьому диску
        public ICollection<FileMetadata> Files { get; set; }
    }
}
