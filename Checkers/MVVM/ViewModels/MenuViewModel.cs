using Checkers.Commands;
using Checkers.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Checkers.MVVM.ViewModels
{
    public class MenuViewModel:ViewModelBase
    {
        public ICommand NavigateToGame { get; }

        public ICommand NavigateToSettings { get; }

        public ICommand NavigateToAbout { get; }

        public MenuViewModel(Navigation navigation, Func<BoardViewModel> createBoardViewModel,
            Func<SettingsViewModel> createSettingsViewModel,Func<AboutViewModel> createAboutViewModel)
        {
            NavigateToGame = new NavigateCommand(navigation, createBoardViewModel);
            NavigateToSettings = new NavigateCommand(navigation, createSettingsViewModel);
            NavigateToAbout = new NavigateCommand(navigation, createAboutViewModel);
        }
    }
}
