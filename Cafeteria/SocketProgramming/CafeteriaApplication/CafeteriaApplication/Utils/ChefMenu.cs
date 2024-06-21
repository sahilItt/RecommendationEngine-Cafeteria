using System.Text.Json;
using CafeteriaApplication.Models;

namespace CafeteriaApplication.Utils
{
    public class ChefMenu
    {
        private StreamWriter writer;
        private StreamReader reader;
        private AdminMenu adminMenu;

        public ChefMenu(StreamWriter writer, StreamReader reader)
        {
            this.writer = writer;
            this.reader = reader;
            adminMenu = new(writer, reader);
        }

        public void ShowChefMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Chef Menu:");
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. View Recommendation");
                Console.WriteLine("3. Send Menu for Next Day");
                Console.WriteLine("4. Logout");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        adminMenu.ViewMenuItems();
                        break;
                    case "2":
                        ViewRecommendationItems();
                        break;
                    case "3":
                        SendNextDayMenu();
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

        private void ViewRecommendationItems()
        {
            ChefRequest request = new ChefRequest { Action = "readRecommendationMenu" };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<ChefResponse>(responseJson);
            if (response.Success)
            {
                // Sort the items by sentiment score in descending order
                var sortedItems = response.RecommendedMenuItems
                    .OrderByDescending(item => item.SentimentScore)
                    .ToList();

                foreach (var item in sortedItems)
                {
                    Console.WriteLine($"Name: {item.MenuItem}, Score: {item.SentimentScore}");
                }
            }
            else
            {
                Console.WriteLine(response.Message);
            }
        }

        private void SendNextDayMenu()
        {
            Console.WriteLine("Enter item id:");
            int itemId = Convert.ToInt32(Console.ReadLine());

            ChefRequest request = new ChefRequest { Action = "create", ItemId = itemId };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<AdminResponse>(responseJson);
            Console.WriteLine(response.Message);
        }

        private void Logout()
        {
            ChefRequest request = new ChefRequest { Action = "logout" };
            writer.WriteLine(JsonSerializer.Serialize(request));
            string response = reader.ReadLine();

            if (response.ToLower() == "logged out")
            {
                Console.WriteLine("Logged out successfully.");
            }
        }
    }
}
