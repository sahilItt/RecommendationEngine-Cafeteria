using System.Text.Json;
using System.Transactions;
using System.Xml.Linq;
using CafeteriaApplication.Models;

namespace CafeteriaApplication.Utils
{
    public class MenuHelper
    {
        public static void ViewMenuItems(StreamWriter writer, StreamReader reader, object request)
        {
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<JsonElement>(responseJson);

            if (response.GetProperty("Success").GetBoolean())
            {
                Console.WriteLine(new string('-', 100));
                Console.WriteLine("| {0, -5} | {1, -20} | {2, -15} | {3, -15} | {4, -20} |", "ID", "Name", "Price (INR)", "Category", "Date Created");
                Console.WriteLine(new string('-', 100));
                foreach (var item in response.GetProperty("MenuItems").EnumerateArray())
                {
                    Console.WriteLine("| {0, -5} | {1, -20} | {2, -15} | {3, -15} | {4, -20} |",
                        item.GetProperty("ItemId").GetInt32(),
                        item.GetProperty("Name").GetString(),
                        item.GetProperty("Price").GetInt32(),
                        item.GetProperty("Category").GetString(),
                        item.GetProperty("DateCreated").GetDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
                }
                Console.WriteLine(new string('-', 100));
            }
            else
            {
                Console.WriteLine(response.GetProperty("Message").GetString());
            }
        }

        public static void DiscardMenuItems(StreamWriter writer, StreamReader reader, object request)
        {
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<JsonElement>(responseJson);

            if (response.GetProperty("Success").GetBoolean())
            {
                  
                var discardMenuArray = JsonDocument.Parse(response.GetProperty("DiscardMenu").ToString()).RootElement;

                if (discardMenuArray.ValueKind == JsonValueKind.Array && discardMenuArray.GetArrayLength() > 0)
                {
                    Console.WriteLine("Total discarded items in menu: " + discardMenuArray.GetArrayLength());
                    Console.WriteLine("{0, -20} | {1, -15} | {2, -15}", "Food Item", "Average Rating", "Sentiments");
                    Console.WriteLine(new string('-', 90));

                    foreach (var discardMenu in discardMenuArray.EnumerateArray())
                    {
                        string foodItem = discardMenu.GetProperty("FoodItem").GetString();
                        double averageRating = discardMenu.GetProperty("AverageRating").GetDouble();
                        string sentiments = discardMenu.GetProperty("Sentiments").GetString();

                        Console.WriteLine("{0, -20} | {1, -15} | {2, -15}", foodItem, averageRating, sentiments);
                    }

                    DiscardItemOptions(writer, reader, request);
                }
                else
                {
                    Console.WriteLine("No Item available in discard menu");
                }
            }
            else
            {
                Console.WriteLine(response.GetProperty("Message").GetString());
            }
        }

        private static void DiscardItemOptions(StreamWriter writer, StreamReader reader, object request)
        {
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Remove Food Item from Menu List (Should be done once a month)");
            Console.WriteLine("2. Get Detailed Feedback (Should be done once a month)");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    RemoveDiscardFoodItem(writer, reader, request);
                    break;
                case 2:
                    GetDetailedFeedback(writer, reader, request);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select 1 or 2.");
                    break;
            }
        }

        private static void RemoveDiscardFoodItem(StreamWriter writer, StreamReader reader, object request)
        {
            Console.WriteLine("Removing a food item from the menu list...");
            Console.WriteLine("Enter food item name to remove: ");
            string removableFoodItemName = Console.ReadLine();

            request = new { Action = "removeDiscardItem", Name = removableFoodItemName };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<JsonElement>(responseJson);
            Console.WriteLine(response.GetProperty("Message").GetString());
        }

        private static void GetDetailedFeedback(StreamWriter writer, StreamReader reader, object request)
        {
            Console.WriteLine("Getting detailed feedback...");
            Console.WriteLine("Please enter the name of the food item for feedback:");
            string feedbackFoodItem = Console.ReadLine();

            request = new { Action = "getDetailedFeedback", Name = feedbackFoodItem };
            writer.WriteLine(JsonSerializer.Serialize(request));

            string responseJson = reader.ReadLine();
            var response = JsonSerializer.Deserialize<JsonElement>(responseJson);
            Console.WriteLine(response.GetProperty("Message").GetString());
        }
    }
}
