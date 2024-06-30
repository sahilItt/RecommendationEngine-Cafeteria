using MySql.Data.MySqlClient;
using ServerApplication.Models;
using System.Data;
using System.Text.Json;
using System.Xml.Linq;

namespace ServerApplication.Services
{
    public class EmployeeService
    {
        private DbHandler dbHandler;

        public EmployeeService(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        public List<MenuNotification> GetMenuNotifications()
        {
            string query = "SELECT message, type FROM notification ORDER BY notificationid";
            List<MenuNotification> menuNotifications = new List<MenuNotification>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    string message = reader.GetString("message");
                    string type = reader.GetString("type");
                    List<MenuNotificationItem> items = JsonSerializer.Deserialize<List<MenuNotificationItem>>(message);

                    menuNotifications.Add(new MenuNotification
                    {
                        Type = type,
                        Items = items
                    });
                }
            }

            return menuNotifications;
        }

        public bool AddItemFeedback(int itemId, int foodRating, string comment, string userEmail)
        {
            string selectQuery = "SELECT name FROM menu_item WHERE itemid = @ItemId";
            MySqlParameter[] selectParameters = {
                new MySqlParameter("@ItemId", itemId)
            };

            DataTable result = dbHandler.ExecuteQuery(selectQuery, selectParameters);
            if (result.Rows.Count == 0)
            {
                return false;
            }

            string feedbackItemName = result.Rows[0]["name"].ToString();

            string insertQuery = "INSERT INTO feedback (feedback_date, comment, food_rating, feedback_person, feedback_item, ItemId) VALUES (NOW(), @Comment, @FoodRating, @UserEmail, @FeedbackItem, @ItemId)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Comment", comment),
                new MySqlParameter("@FoodRating", foodRating),
                new MySqlParameter("@UserEmail", userEmail),
                new MySqlParameter("@FeedbackItem", feedbackItemName),
                new MySqlParameter("@ItemId", itemId)
            };

            return dbHandler.ExecuteNonQuery(insertQuery, parameters) > 0;
        }
    }
}
