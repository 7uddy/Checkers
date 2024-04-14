using Checkers.Commands;
using Checkers.Stores;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;

namespace Checkers.MVVM.ViewModels
{
    public class AboutViewModel:ViewModelBase
    {
        public static int WhiteWins { get; set; }
        public static int MaximumWhitePieces { get; set; }
        public static int RedWins { get; set; }
        public static int MaximumRedPieces { get; set; }
        public ICommand NavigateToMenu { get; }
        public AboutViewModel(Navigation navigation, Func<MenuViewModel> createMenuViewModel)
        {
            NavigateToMenu = new NavigateCommand(navigation, createMenuViewModel);
            ReadWins();
        }
        public static void ReadWins()
        {
            string filePath = "../../JSONs/stats.json";
            if (!File.Exists(filePath))
            {
                WhiteWins = 0;
                MaximumWhitePieces = 0;
                RedWins = 0;
                MaximumRedPieces = 0;
                return;
            }
            string jsonText = File.ReadAllText(filePath);
            try
            {
                int[] integers = JsonConvert.DeserializeObject<int[]>(jsonText);

                if (integers.Length == 4)
                {
                    WhiteWins = integers[0];
                    MaximumWhitePieces = integers[1];
                    RedWins = integers[2];
                    MaximumRedPieces = integers[3];
                }
            }
            catch (JsonException)
            {
                Console.WriteLine("Error deserializing JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
