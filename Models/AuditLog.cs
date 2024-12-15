namespace DiskSpaceMonitor.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } // Дія: "Created", "Updated", "Deleted"
        public DateTime Timestamp { get; set; } // Коли дія була виконана
        public int FileId { get; set; } // Зв'язок із файлом
        public FileMetadata File { get; set; } // Відношення до файлу
        public string UserName { get; set; } // Користувач, що здійснив дію
    }
}
