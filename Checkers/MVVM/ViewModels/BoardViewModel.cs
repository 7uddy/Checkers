using Caliburn.Micro;
using Checkers.Models;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;
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
        public Player CurrentPlayer { get; private set; }

        private BindableCollection<BindableCollection<Cell>> _squares;

        public BindableCollection<BindableCollection<Cell>> Squares
        {
            get { return _squares; }
            set
            {
                if (_squares != value)
                {
                    _squares = value;
                    OnPropertyChanged(nameof(Squares));
                }
            }
        }

        public BoardViewModel()
        {
            CurrentPlayer=Player.Red;
            _squares = Board.GetInitialCells();
        }
        public Cell SimpleCell { get; set; }
        private List<Cell> GreenCells { get; set; }

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                if (_clickCommand == null)
                {
                    _clickCommand = new DelegateCommand<Cell>(OnCellClicked);
                }
                return _clickCommand;
            }
        }

        public void OnCellClicked(Cell clickedCell)
        {
            //New position selected
            if(clickedCell.ImagePath== "../../Resources/Green.png")
            {
                MoveToNewCell(clickedCell);
                DeleteGreen();
                CurrentPlayer = CurrentPlayer == Player.Red ? Player.White : Player.Red;
            }

            //Piece selected
            if (clickedCell.Piece != null && clickedCell.Piece.Color == CurrentPlayer)
            {
                SimpleCell = clickedCell;
                DeleteGreen();
                SetAcceptedMoves(clickedCell);
            }
            else
            {
                DeleteGreen();
            }
            Squares = new BindableCollection<BindableCollection<Cell>>(_squares);
        }

        private void MoveToNewCell(Cell clickedCell)
        {

            clickedCell.Piece = SimpleCell.Piece;
            clickedCell.ImagePath = SimpleCell.ImagePath;
            SimpleCell.Piece = null;
            SimpleCell.ImagePath = "../../Resources/transparent.png";

        }

        void DeleteGreen()
        {
            if (GreenCells!=null)
            {
                foreach (var cell in GreenCells)
                {
                    if(cell.Piece==null) cell.ImagePath = "../../Resources/transparent.png";
                }
            }
        }
        void SetAcceptedMoves(Cell clickedCell)
        {
            List<Position> greenPositions = new List<Position>();
            if (clickedCell.Piece.Color == Player.White)
            {
                greenPositions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthWest));
                greenPositions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthEast));
            }
            else
            {
                greenPositions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthWest));
                greenPositions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthEast));
            }
            GreenCells = new List<Cell>();
            foreach (var pos in greenPositions)
            {
                foreach (var row in _squares)
                {
                    foreach (var cell in row)
                    {
                        if (cell.CellPosition == pos)
                        {
                            cell.ImagePath = "../../Resources/Green.png";
                            GreenCells.Add(cell);
                        }
                    }
                }
            } 
        }
        
    }
}
