namespace ServerApplication.Models
{
    public class LoginResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
    }
}
