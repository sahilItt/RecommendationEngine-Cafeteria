namespace ServerApplication.Models
{
    public class EmployeeResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<MenuNotification>? MenuNotifications { get; set; }
    }

    public class MenuNotification
    {
        public int VoteYes { get; set; }
        public int VoteNo { get; set; }
        public string? Type { get; set; }
        public List<MenuNotificationItem>? Items { get; set; }
        public string? RecommendationMessage { get; set; }
    }

    public class MenuNotificationItem
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public float Price { get; set; }
        public string? Category { get; set; }
        public string? SweetTooth { get; set; }
        public string? DietaryType { get; set; }
        public string? SpiceLevel { get; set; }
        public string? FoodType { get; set; }
    }
}
