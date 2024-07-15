namespace ServerApplication.Models
{
    public class AdminRequest
    {
        public string? Action { get; set; }
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public float Price { get; set; }
        public string? Category { get; set; }
        public DateOnly Date { get; set; }
        public string? Email { get; set; }
    }
}
