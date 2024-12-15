namespace DiskSpaceMonitor.Models
{
    public class FolderDataRequest
    {
        public string ServerName { get; set; } // Имя сервера, который отправляет данные
        public List<FolderInfoDto> FolderInfoList { get; set; } // Список данных о папках
    }
}
