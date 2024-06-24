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

        public string SetMenuItemsNotification(List<int> itemIds)
        {
            if (itemIds == null || itemIds.Count == 0)
            {
                return JsonSerializer.Serialize(new { Message = "No item IDs provided." });
            }

            string query = $"SELECT name, price, category FROM menu_item WHERE itemid IN ({string.Join(",", itemIds)}) ORDER BY itemid";
            var menuItems = new List<object>();

            using (MySqlDataReader reader = dbHandler.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var item = new
                    {
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
