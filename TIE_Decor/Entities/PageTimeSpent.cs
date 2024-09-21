namespace TIE_Decor.Entities
{
    public class PageTimeSpent
    {
        public int Id { get; set; }
        public string PageUrl { get; set; }
        public int TimeSpent { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
