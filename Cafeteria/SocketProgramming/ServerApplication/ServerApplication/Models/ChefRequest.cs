namespace ServerApplication.Models
{
    public class ChefRequest
    {
        public string? Action {  get; set; }
        public List<int> ItemIds { get; set; } = new List<int>();
        public string? ChefMenuNotificationMessage { get; set; }
    }
}
