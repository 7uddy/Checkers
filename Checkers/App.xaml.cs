using Checkers.MVVM.ViewModels;
using Checkers.Stores;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Navigation _navigation = new Navigation();
        protected override void OnStartup(StartupEventArgs e)
        {
            _navigation.CurrentViewModel = CreateMenuViewModel();
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigation)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }

        private MenuViewModel CreateMenuViewModel()
        {
            return new MenuViewModel(_navigation,CreateBoardViewModel,CreateSettingsViewModel,CreateAboutViewModel);
        }

        private BoardViewModel CreateBoardViewModel()
        {
            return new BoardViewModel(_navigation,CreateMenuViewModel);
        }

        private SettingsViewModel CreateSettingsViewModel()
        {
            return new SettingsViewModel(_navigation,CreateMenuViewModel);
        }

        private AboutViewModel CreateAboutViewModel()
        {
            return new AboutViewModel(_navigation,CreateMenuViewModel);
        }
    }
}
