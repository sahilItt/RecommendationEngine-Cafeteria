using CafeteriaApplication.Models;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace CafeteriaApplication.Utils
{
    public static class MenuHelper
    {
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
    }
}
