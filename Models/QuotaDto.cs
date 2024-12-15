namespace DiskSpaceMonitor.Models
{
    public class QuotaDto
    {
        public string Path { get; set; }
        public decimal TotalSize { get; set; }
        public decimal FreeSize { get; set; }
        public decimal UsageSize { get; set; }
        public string ServerName { get; set; } // Получаем название сервера вместо ID
        public DateTime? GrowthTime { get; set; }
    }

}
