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

        public List<MenuNotification> GetMenuNotifications(string notificationType)
        {
            string query = "select message, type, vote_yes, vote_no from notification where type = @NotificationType order by notification_date desc limit 1;";
            MySqlParameter[] parameters = {
                new MySqlParameter("@NotificationType", notificationType)
            };
            List<MenuNotification> menuNotifications = new List<MenuNotification>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query, parameters))
            {
                while (reader.Read())
                {
                    string message = reader.GetString("message");
                    string type = reader.GetString("type");
                    int voteYes = reader.GetInt32("vote_yes");
                    int voteNo = reader.GetInt32("vote_no");
                    List<MenuNotificationItem> items = JsonSerializer.Deserialize<List<MenuNotificationItem>>(message);

                    menuNotifications.Add(new MenuNotification
                    {
                        VoteYes = voteYes,
                        VoteNo = voteNo,
                        Type = type,
                        Items = items
                    });
                }
            }

            return menuNotifications;
        }

        public bool AddMenuVotes(bool hasLikedMenu)
        {
            string query;
            if(hasLikedMenu)
            {
                query = "UPDATE notification SET vote_yes = vote_yes + 1 ORDER BY notification_date DESC LIMIT 1;";
            }
            else
            {
                query = "UPDATE notification SET vote_no = vote_no + 1 ORDER BY notification_date DESC LIMIT 1;";
            }

            return dbHandler.ExecuteNonQuery(query) > 0;
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
