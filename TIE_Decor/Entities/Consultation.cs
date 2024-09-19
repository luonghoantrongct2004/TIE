namespace TIE_Decor.Entities;

public class Consultation
{
    public int ConsultationId { get; set; }  // ID của buổi tư vấn
    public int UserId { get; set; }  // ID của người dùng (chủ nhà)
    public int DesignerId { get; set; }  // ID của nhà thiết kế
    public DateTime ScheduledTime { get; set; }  // Thời gian diễn ra buổi tư vấn
    public string Status { get; set; } = "Đã đặt lịch";  // Trạng thái của buổi tư vấn (ví dụ: "Đã đặt lịch", "Hoàn thành", "Hủy")
    public string? Notes { get; set; }  // Ghi chú bổ sung về buổi tư vấn (cả người dùng và nhà thiết kế có thể thêm)

    // Quan hệ với ApplicationUser (người dùng)
    public virtual User? User { get; set; }

    // Quan hệ với InteriorDesigner (nhà thiết kế)
    public virtual InteriorDesigner? Designer { get; set; }
}

