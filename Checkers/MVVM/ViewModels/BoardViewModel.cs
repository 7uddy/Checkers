using Caliburn.Micro;
using Checkers.Models;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Checkers.MVVM.ViewModels
{
    public class BoardViewModel:ViewModelBase
    {
        //GameLogic _gameLogic;
        //public BindableCollection<BindableCollection<SquareViewModel>> Squares { get; set; }
        
        //public GameTableViewModel(GameLogic gameLogic)
        //{
        //    this._gameLogic = gameLogic;
            
        //}

        public Player CurrentPlayer { get; private set; }

        private BindableCollection<BindableCollection<Cell>> _squares;

        public BindableCollection<BindableCollection<Cell>> Squares
        {
            get { return _squares; }
        }

        public BoardViewModel()
        {
            CurrentPlayer=Player.White;
            _squares = Board.GetInitialCells();
        }
        public Cell SimpleCell { get; set; }

        private ICommand clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                if (clickCommand == null)
                {
                    clickCommand = new RelayCommand<Cell>(Hello);
                }
                return clickCommand;
            }
        }
        public void Hello(Cell obj)
        {
               Console.WriteLine("Hello");
        }
    }
}
