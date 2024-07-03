using System.Text.Json;
using MySql.Data.MySqlClient;

namespace ServerApplication.Services
{
    public static class ServicesHelper
    {
        public static string FetchDiscardMenu(DbHandler dbHandler)
        {
            string query = "SELECT food_item, average_rating FROM discard_menu";
            var discardMenu = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var menuItem = new
                    {
                        FoodItem = reader.GetString("food_item"),
                        AverageRating = reader.GetDouble("average_rating")
                    };

                    discardMenu.Add(menuItem);
                }
            }

            return JsonSerializer.Serialize(discardMenu);
        }

        private static List<(string, double)> FetchDiscardRecommendationItems(DbHandler dbHandler)
        {
            string query = "select recommended_items, sentiment_score from food_recommendation where sentiment_score <= 2";
            List<(string, double)> recommendations = new List<(string, double)>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    string recommendedItems = reader.GetString("recommended_items");
                    double sentimentScore = reader.GetDouble("sentiment_score");
                    recommendations.Add((recommendedItems, sentimentScore));
                }
            }

            return recommendations;
        }

        public static string ViewDiscardItems(DbHandler dbHandler)
        {
            string query = "select food_item, average_rating from discard_menu";
            var discardMenu = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var menuItem = new
                    {
                        FoodItem = reader.GetString("food_item"),
                        AverageRating = reader.GetDouble("average_rating")
                    };

                    discardMenu.Add(menuItem);
                }
            }

            return JsonSerializer.Serialize(discardMenu);
        }

        public static void AddDiscardItems(DbHandler dbHandler)
        {
            List<(string, double)> discardedRecommendationItems = FetchDiscardRecommendationItems(dbHandler);
            foreach (var recommendation in discardedRecommendationItems)
            {
                if (!IsItemInDiscardMenu(recommendation.Item1, dbHandler))
                {
                    string query = "INSERT INTO discard_menu (food_item, average_rating, date_added) VALUES (@FoodItem, @AverageRating, NOW())";

                    MySqlParameter[] parameters = {
                        new MySqlParameter("@FoodItem", recommendation.Item1),
                        new MySqlParameter("@AverageRating", recommendation.Item2)
                    };

                    dbHandler.ExecuteNonQuery(query, parameters);
                }  
            }
        }

        private static bool IsItemInDiscardMenu(string foodItem, DbHandler dbHandler)
        {
            string query = "SELECT COUNT(*) FROM discard_menu WHERE food_item = @FoodItem";

            MySqlParameter[] parameters = {
                new MySqlParameter("@FoodItem", foodItem)
            };

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query, parameters))
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0) > 0;
                }
            }

            return false;
        }
    }
}
