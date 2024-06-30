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
        public string? Type { get; set; }
        public List<MenuNotificationItem>? Items { get; set; }
    }

    public class MenuNotificationItem
    {
        public string? ItemName { get; set; }
        public float Price { get; set; }
        public string? Category { get; set; }
    }
}
