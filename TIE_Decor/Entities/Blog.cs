namespace TIE_Decor.Entities;

public class Blog
{
    public int Id { get; set; } // Khóa chính (Primary Key)

    public string Title { get; set; } // Tiêu đề của blog

    public string Content { get; set; } // Nội dung của blog

    public string Author { get; set; } // Tên tác giả hoặc Id tác giả

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    public string? ImageUrl { get; set; }

}

