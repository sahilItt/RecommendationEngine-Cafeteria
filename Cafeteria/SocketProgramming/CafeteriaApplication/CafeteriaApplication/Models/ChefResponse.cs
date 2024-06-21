namespace CafeteriaApplication.Models
{
    public class ChefResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<RecommendedItem>? RecommendedMenuItems { get; set; }
    }
}
