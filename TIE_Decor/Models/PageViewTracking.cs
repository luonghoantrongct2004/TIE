namespace TIE_Decor.Models
{
    public class PageViewTracking
    {
        public int Id { get; set; }
        public string PageUrl { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
