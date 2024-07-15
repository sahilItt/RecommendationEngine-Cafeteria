namespace CafeteriaApplication.Models
{
    public class User
    {
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class Profile
    {
        public string? VegetarianPreference { get; set; }
        public string? SpiceLevel { get; set; }
        public string? FoodPreference { get; set; }
        public string? SweetTooth { get; set; }
    }

    public class DiscardItemFeedback
    {
        public string Question1 { get; set; }
        public string Answer1 { get; set; }
        public string Question2 { get; set; }
        public string Answer2 { get; set; }
        public string Question3 { get; set; }
        public string Answer3 { get; set; }
    }
}
