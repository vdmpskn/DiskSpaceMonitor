namespace DiskSpaceMonitor.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string Company { get; set; } // Компания
        public string Service { get; set; } // Служба
        public string Department { get; set; } // Отдел
        public string Sector { get; set; } // Сектор
        public long Size { get; set; } // Размер папки
        public int ServerId { get; set; } // ID сервера
        public Server Server { get; set; } // Навигационное свойство

        public DateTime GrowthDate { get; set; }
    }

}
