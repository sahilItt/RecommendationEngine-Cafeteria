using System.Text.Json;
using MySql.Data.MySqlClient;
using ServerApplication.Models;

namespace ServerApplication.Services
{
    public class AdminService
    {
        private DbHandler dbHandler;

        public AdminService(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        public List<MenuItem> GetMenuItems()
        {
            string query = "SELECT itemid, name, price, category, date_created FROM menu_item ORDER BY itemid";
            List<MenuItem> menuItems = new List<MenuItem>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    menuItems.Add(new MenuItem
                    {
                        ItemId = reader.GetInt32("itemid"),
                        Name = reader.GetString("name"),
                        Price = reader.GetInt32("price"),
                        Category = reader.GetString("category"),
                        DateCreated = reader.GetDateTime("date_created")
                    });
                }
            }

            return menuItems;
        }

        public bool AddMenuItem(string name, float price, string category)
        {
            string query = "INSERT INTO menu_item (name, price, category, date_created) VALUES (@Name, @Price, @Category, NOW())";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Name", name),
                new MySqlParameter("@Price", price),
                new MySqlParameter("@Category", category)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateMenuItem(int itemId, string name, float price, string category)
        {
            string query = "UPDATE menu_item SET name = @Name, price = @Price, category = @Category WHERE itemid = @ItemId";
            MySqlParameter[] parameters = {
                new MySqlParameter("@ItemId", itemId),
                new MySqlParameter("@Name", name),
                new MySqlParameter("@Price", price),
                new MySqlParameter("@Category", category)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteMenuItem(int itemId)
        {
            string query = "DELETE FROM menu_item WHERE itemid = @ItemId";
            MySqlParameter[] parameters = {
                new MySqlParameter("@ItemId", itemId)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool SaveAdminMenuNotification(string notificationMessage)
        {
            string query = "INSERT INTO notification (message, type, notification_date, vote_yes, vote_no) VALUES (@NotificationMessage, 'admin', NOW(), 0, 0)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@NotificationMessage", notificationMessage)
            };

            return dbHandler.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
