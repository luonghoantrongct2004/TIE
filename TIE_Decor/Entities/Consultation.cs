        namespace TIE_Decor.Entities;

public class Consultation
{
    public int ConsultationId { get; set; }  
    public Guid UserId { get; set; }
    public Guid DesignerID { get; set; }
    public DateTime ScheduledTime { get; set; }  
    public string Status { get; set; } = "Đã đặt lịch"; 
    public string? Notes { get; set; } 

    public virtual User? User { get; set; }
}

