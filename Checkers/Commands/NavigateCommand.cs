using Checkers.MVVM.ViewModels;
using Checkers.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Commands
{
    public class NavigateCommand : CommandBase
    {
        private readonly Navigation _navigation;
        private readonly Func<ViewModelBase> _createViewModel;
        public NavigateCommand(Navigation navigation, Func<ViewModelBase> createViewModel)
        {
            _navigation = navigation;
            _createViewModel = createViewModel;
        }
        public override void Execute(object parameter)
        {
            _navigation.CurrentViewModel = _createViewModel();
        }
    }
}
