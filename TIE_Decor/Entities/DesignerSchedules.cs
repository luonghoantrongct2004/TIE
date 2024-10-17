using System.ComponentModel.DataAnnotations;

namespace TIE_Decor.Entities
{
    public class DesignerSchedules
    {
        [Key]
        public int ScheduleId { get; set; } 
        public Guid DesignerId { get; set; } 
        public DateTime ScheduledTime { get; set; } 
        public string? Status { get; set; } = "";
        public string? Notes { get; set; }

    }
}
