namespace ServerApplication.Models
{
    public class ChefResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? MenuVotes { get; set; }
        public List<RecommendedItem>? RecommendedMenuItems { get; set; }
        public List<FullMenuItem>? FullMenuItems { get; set; }
    }

    public class FullMenuItem
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public int Price { get; set; }
        public string? Category { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
