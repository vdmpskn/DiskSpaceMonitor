namespace DiskSpaceMonitor.Models
{
    public class FolderInfoDto
    {
        public string Company { get; set; } // Название компании
        public string Service { get; set; } // Название службы
        public string Department { get; set; } // Название отдела
        public string Sector { get; set; } // Название сектора
        public long Size { get; set; } // Размер папки в байтах
    }

}
