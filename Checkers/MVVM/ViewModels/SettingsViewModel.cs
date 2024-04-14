using Checkers.Commands;
using Checkers.Stores;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;

namespace Checkers.MVVM.ViewModels
{
    public class SettingsViewModel:ViewModelBase
    {
        private static readonly string _settingsPath = "../../JSONs/settings.json";

        private static bool _isMultiJumpToggled;
        public static bool IsMultiJumpToggled
        {
            get 
            { 
            return _isMultiJumpToggled; 
            }
            set
            {
                if (_isMultiJumpToggled != value)
                {
                    _isMultiJumpToggled = value;
                    WriteSettings();
                }
            }
        }

        public ICommand NavigateToMenu { get; }

        public SettingsViewModel(Navigation navigation,Func<MenuViewModel>createMenuViewModel)
        { 
            ReadSettings();
            NavigateToMenu = new NavigateCommand(navigation, createMenuViewModel);
        }

        private static void WriteSettings()
        {
            if (File.Exists(_settingsPath))
            {
                File.Delete(_settingsPath);
            }
            int multiJump = IsMultiJumpToggled ? 1 : 0;
            try
            {
                string jsonText = JsonConvert.SerializeObject(multiJump);

                File.WriteAllText(_settingsPath, jsonText);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public static void ReadSettings() 
        {
            if (!File.Exists(_settingsPath))
            {
                return;
            }
            string jsonText = File.ReadAllText(_settingsPath);
            try
            {
                int multiJump = int.Parse(jsonText);
                IsMultiJumpToggled = multiJump == 1;
            }
            catch (JsonException)
            {
                Console.WriteLine("Error deserializing settings JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred on settings JSON: " + ex.Message);
            }
        }
    }
}
