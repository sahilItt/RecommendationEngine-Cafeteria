namespace CafeteriaApplication.Models
{
    public class ChefResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? MenuVotes { get; set; }
        public List<RecommendedItem>? RecommendedMenuItems { get; set; }
        public List<dynamic>? FullMenuItems { get; set; }
        public List<string> DiscardMenuItemFeedback { get; set; }
    }
}
