namespace ServerApplication.Models
{
    public class ChefRequest
    {
        public string? Action {  get; set; }
        public int ItemId { get; set; }
        public string? NotificationMessage { get; set; }
    }
}
