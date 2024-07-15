namespace ServerApplication.Models
{
    public class User
    {
        public int PersonId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class Profile
    {
        public string? VegetarianPreference { get; set; }
        public string? SpiceLevel { get; set; }
        public string? FoodPreference { get; set; }
        public string? SweetTooth { get; set; }
    }
}
