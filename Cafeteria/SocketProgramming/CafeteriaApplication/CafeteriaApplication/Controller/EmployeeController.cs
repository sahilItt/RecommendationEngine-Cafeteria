using System.Text.Json;
using CafeteriaApplication.Models;

namespace CafeteriaApplication.Controller
{
    public class EmployeeController
    {
        private StreamWriter writer;
        private StreamReader reader;

        public EmployeeController(StreamWriter writer, StreamReader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public void ShowEmployeeMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Employee Menu:");
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
            Console.Write("Enter the notification type (1 for Admin, any other key for Chef): ");
            string notificationType = Console.ReadLine() == "1" ? "admin" : "chef";

            EmployeeRequest request = new EmployeeRequest { Action = "readNotification", NotificationType = notificationType };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<EmployeeResponse>(responseJson);

            if (response.Success)
            {
                foreach (var menuNotification in response.MenuNotifications)
                {
                    Console.WriteLine($"Notification Type: {menuNotification.Type}");
                    Console.WriteLine("{0, -10} | {1, -30} | {2, -10} | {3, -10}", "ItemId", "ItemName", "Price (INR)", "Category");
                    Console.WriteLine(new string('-', 90));

                    foreach (var item in menuNotification.Items)
                    {

                        Console.WriteLine("{0, -10} | {1, -30} | {2, -10} | {3, -10}",
                        item.ItemId,
                        item.ItemName,
                        item.Price,
                        item.Category);
                    }

                    Console.WriteLine(new string('-', 90));
                    Console.WriteLine("Total Votes for menu: ");
                    Console.WriteLine("{0, -10} | {1, -10}", "VoteYes", "VoteNo");
                    Console.WriteLine("{0, -10} | {1, -10}", menuNotification.VoteYes, menuNotification.VoteNo);
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void VoteItems()
        {
            Console.Write("Do you like the menu? (yes/no): ");
            string feedback;
            while (true)
            {
                feedback = Console.ReadLine()?.Trim().ToLower();
                if (feedback == "yes" || feedback == "no")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                    Console.Write("Do you like the menu? (yes/no): ");
                }
            }

            EmployeeRequest request = new EmployeeRequest
            {
                Action = "submitVote",
                HasLikedMenu = feedback == "yes"
            };

            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<EmployeeResponse>(responseJson);

            Console.WriteLine(response.Message);
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
