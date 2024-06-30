using System.Text.Json;
using CafeteriaApplication.Models;

namespace CafeteriaApplication.Utils
{
    public class EmployeeMenu
    {
        private StreamWriter writer;
        private StreamReader reader;

        public EmployeeMenu(StreamWriter writer, StreamReader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void ShowEmployeeMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Chef Menu:");
                Console.WriteLine("1. View Notification");
                Console.WriteLine("2. Vote Items");
                Console.WriteLine("3. Give Feedback & Rating");
                Console.WriteLine("4. Logout");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewNotification();
                        break;
                    case "2":
                        VoteItems();
                        break;
                    case "3":
                        GiveFeedback();
                        break;
                    case "4":
                        Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }

        private void ViewNotification()
        {
            EmployeeRequest request = new EmployeeRequest { Action = "readNotification" };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<EmployeeResponse>(responseJson);

            if(response.Success)
            {
                Console.WriteLine("{0, -5} | {1, -30} | {2, -10}", "ItemName", "Price (INR)", "Category");
                Console.WriteLine(new string('-', 90));
                foreach (var menuNotificationItem in response.MenuNotifications)
                {
                    foreach (var item in menuNotificationItem.Items)
                    {
                        Console.WriteLine("{0, -5} | {1, -30} | {2, -10}",
                        item.ItemName,
                        item.Price,
                        item.Category);
                    }
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void VoteItems() 
        {
        }

        private void GiveFeedback()
        {
            Console.Write("Enter the ItemId: ");
            int itemId;
            while (!int.TryParse(Console.ReadLine(), out itemId))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                Console.Write("Enter the ItemId: ");
            }

            Console.Write("Enter your comment: ");
            string comment = Console.ReadLine();

            Console.Write("Enter the food rating (1-5): ");
            int foodRating;
            while (!int.TryParse(Console.ReadLine(), out foodRating) || foodRating < 1 || foodRating > 5)
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                Console.Write("Enter the food rating (1-5): ");
            }

            EmployeeRequest request = new EmployeeRequest
            {
                Action = "createFeedback",
                Comment = comment,
                FoodRating = foodRating,
                ItemId = itemId
            };

            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<EmployeeResponse>(responseJson);

            Console.WriteLine(response.Message);
        }

        private void Logout()
        {
            EmployeeRequest request = new EmployeeRequest { Action = "logout" };
            writer.WriteLine(JsonSerializer.Serialize(request));
            string response = reader.ReadLine();

            if (response.ToLower() == "logged out")
            {
                Console.WriteLine("Logged out successfully.");
            }
        }
    }
}
