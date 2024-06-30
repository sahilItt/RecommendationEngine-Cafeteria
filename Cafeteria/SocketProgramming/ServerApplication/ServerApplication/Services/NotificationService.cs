using MySql.Data.MySqlClient;
using System.Text.Json;

namespace ServerApplication.Services
{
    public class NotificationService
    {
        private DbHandler dbHandler;

        public NotificationService(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler; 
        }

        public string SetAdminMenuItemsNotification()
        {
            string query = $"select itemid, name, price, category from menu_item order by itemid desc limit 1;";
            var menuItems = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var item = new
                    {
                        ItemId = reader.GetInt32("itemid"),
                        ItemName = reader.GetString("name"),
                        Price = reader.GetFloat("price"),
                        Category = reader.GetString("category")
                    };

                    menuItems.Add(item);
                }
            }

            return JsonSerializer.Serialize(menuItems);
        }

        public string SetChefMenuItemsNotification(List<int> itemIds)
        {
            if (itemIds == null || itemIds.Count == 0)
            {
                return JsonSerializer.Serialize(new { Message = "No item IDs provided." });
            }

            string query = $"SELECT name, price, category, itemid FROM menu_item WHERE itemid IN ({string.Join(",", itemIds)}) ORDER BY itemid";
            var menuItems = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var item = new
                    {
                        ItemId = reader.GetInt32("itemid"),
                        ItemName = reader.GetString("name"),
                        Price = reader.GetFloat("price"),
                        Category = reader.GetString("category")
                    };

                    menuItems.Add(item);
                }
            }

            return JsonSerializer.Serialize(menuItems);
        }

    }
}
