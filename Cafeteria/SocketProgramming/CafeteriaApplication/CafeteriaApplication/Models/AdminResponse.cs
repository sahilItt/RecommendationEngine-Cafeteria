using System.Data;

namespace CafeteriaApplication.Models
{
    public class AdminResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? DiscardMenu { get; set; }
        public List<dynamic>? MenuItems { get; set; }
    }
}
