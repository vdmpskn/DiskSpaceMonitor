namespace DiskSpaceMonitor.Models
{
    public class FileMetadata
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAccessedDate { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }

        // Зв'язок із сервером
        public int ServerId { get; set; }
        public Server Server { get; set; }

        // Зв'язок із диском
        public int? DiskId { get; set; }
        public Disk Disk { get; set; }

        // Колекція записів в аудит-лог
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}
