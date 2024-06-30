namespace CafeteriaApplication.Models
{
    public class EmployeeRequest
    {
        public string Action { get; set; }
        public string Comment { get; set; }
        public int FoodRating { get; set; }
        public int ItemId { get; set; }
    }
}
