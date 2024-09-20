namespace TIE_Decor.Entities;

public class Consultation
{
    public int ConsultationId { get; set; }  // ID của buổi tư vấn
<<<<<<< HEAD
    public int UserId { get; set; }
    public int DesignerId { get; set; }  // ID của nhà thiết kế
=======

>>>>>>> 13ef6fef4ccb95e86555afb179a15ec4b2d06350
    public DateTime ScheduledTime { get; set; }  // Thời gian diễn ra buổi tư vấn
    public string Status { get; set; } = "Đã đặt lịch";  // Trạng thái của buổi tư vấn (ví dụ: "Đã đặt lịch", "Hoàn thành", "Hủy")
    public string? Notes { get; set; }  // Ghi chú bổ sung về buổi tư vấn (cả người dùng và nhà thiết kế có thể thêm)

    // Quan hệ với ApplicationUser (người dùng)
    public virtual User? User { get; set; }

}

