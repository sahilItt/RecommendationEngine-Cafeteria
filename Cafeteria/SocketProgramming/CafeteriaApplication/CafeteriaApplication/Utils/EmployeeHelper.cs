using CafeteriaApplication.Models;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace CafeteriaApplication.Utils
{
    public static class EmployeeHelper
    {
        public static string GetVegetarianPreference()
        {
            Console.WriteLine("Please select your vegetarian preference:");
            Console.WriteLine("1. Veg");
            Console.WriteLine("2. Non Veg");
            Console.WriteLine("3. Egg");

            int choice = int.Parse(Console.ReadLine());
            return choice switch
            {
                1 => "Veg",
                2 => "Non Veg",
                3 => "Egg",
                _ => throw new ArgumentException("Invalid choice")
            };
        }

        public static string GetSpiceLevel()
        {
            Console.WriteLine("Please select your spice level:");
            Console.WriteLine("1. Mild");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Spicy");

            int choice = int.Parse(Console.ReadLine());
            return choice switch
            {
                1 => "Mild",
                2 => "Medium",
                3 => "Spicy",
                _ => throw new ArgumentException("Invalid choice")
            };
        }

        public static string GetFoodPreference()
        {
            Console.WriteLine("Please select your food preference:");
            Console.WriteLine("1. North Indian");
            Console.WriteLine("2. South Indian");
            Console.WriteLine("3. Other");

            int choice = int.Parse(Console.ReadLine());
            return choice switch
            {
                1 => "North Indian",
                2 => "South Indian",
                3 => "Other",
                _ => throw new ArgumentException("Invalid choice")
            };
        }

        public static string GetSweetTooth()
        {
            Console.WriteLine("Do you have a sweet tooth?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

            int choice = int.Parse(Console.ReadLine());
            return choice switch
            {
                1 => "Yes",
                2 => "No",
                _ => throw new ArgumentException("Invalid choice")
            };
        }
    }
}
