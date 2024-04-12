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
    public class SettingsViewModel:ViewModelBase
    {
        
        private static bool _isMultiJumpToggled;
        public static bool IsMultiJumpToggled
        {
            get { return _isMultiJumpToggled; }
            set
            {
                if (_isMultiJumpToggled != value)
                {
                    _isMultiJumpToggled = value;
                }
            }
        }

        public ICommand NavigateToMenu { get; }

        public SettingsViewModel(Navigation navigation,Func<MenuViewModel>createMenuViewModel)
        { 
            NavigateToMenu = new NavigateCommand(navigation, createMenuViewModel);
            _isMultiJumpToggled = false;
        }
    }
}
