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
        public ICommand NavigateToMenu { get; }
        public AboutViewModel(Navigation navigation, Func<MenuViewModel> createMenuViewModel)
        {
            NavigateToMenu = new NavigateCommand(navigation, createMenuViewModel);
        }
    }
}
