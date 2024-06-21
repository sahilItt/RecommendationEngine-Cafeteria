using System.Data;

namespace ServerApplication.Models
{
    public class AdminResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public List<MenuItem>? MenuItems { get; set; }
    }

    public class MenuItem
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public int Price { get; set; }
        public string? Category { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
