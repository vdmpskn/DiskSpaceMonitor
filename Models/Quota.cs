using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DiskSpaceMonitor.Models
{
    public class Quota
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalSize { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FreeSize { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UsageSize { get; set; }

        public int ServerId { get; set; }
        public Server Server { get; set; }

        public DateTime? GrowthTime { get; set; }
    }
}
