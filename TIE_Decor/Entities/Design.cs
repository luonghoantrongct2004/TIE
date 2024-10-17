namespace TIE_Decor.Entities;

public class Design
{
    public int DesignId { get; set; }  // ID của thiết kế
    public int UserId { get; set; }  // ID của người dùng sở hữu thiết kế
    public string DesignName { get; set; }  // Tên của thiết kế
    public string DesignDetails { get; set; }  // Thông tin chi tiết về thiết kế (có thể là mô tả, hình ảnh)

    // Quan hệ với ApplicationUser (người dùng)
    public virtual User User { get; set; }
}

