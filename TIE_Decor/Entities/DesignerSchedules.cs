using System.ComponentModel.DataAnnotations;

namespace TIE_Decor.Entities
{
    public class DesignerSchedules
    {
        [Key]
        public int ScheduleId { get; set; } // Khóa chính
        public Guid DesignerId { get; set; } // ID của designer
        public DateTime ScheduledTime { get; set; } // Thời gian đã lên lịch
        public string Status { get; set; } // Trạng thái lịch (đã xác nhận, đang chờ, v.v.)
        public string Notes { get; set; } // Ghi chú thêm

        // Có thể thêm các thuộc tính khác tùy theo yêu cầu
    }
}
