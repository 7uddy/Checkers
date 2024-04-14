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
            BoardViewModel.ReadWins();
        }
    }
}
