﻿using Caliburn.Micro;
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
                    _clickCommand = new RelayCommand<Cell>(OnCellClicked);
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

                if(IsCaptureMove(SimpleCell,clickedCell))
                {
                    Cell capturedCell = GetCapturedCell(SimpleCell, clickedCell);
                    capturedCell.Piece = null;
                    capturedCell.ImagePath = "../../Resources/transparent.png";
                }

                IsPromotionMove(clickedCell);
                DeleteGreen();
                CurrentPlayer = CurrentPlayer == Player.Red ? Player.White : Player.Red;
            }

            //Piece selected
            else if (clickedCell.Piece != null && clickedCell.Piece.Color == CurrentPlayer)
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
            if (clickedCell.Piece.IsKing == false)
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
            else
            {
                List<Position> greenPositions = new List<Position>
                {
                    clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthWest),
                    clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthEast),
                    clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthWest),
                    clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthEast)
                };
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
        private bool IsCaptureMove(Cell from,Cell to)
        {
            return Math.Abs(from.CellPosition.Row - to.CellPosition.Row) == 2;
        }

        private Cell GetCapturedCell(Cell from, Cell to)
        {
            int row = (from.CellPosition.Row + to.CellPosition.Row) / 2;
            int col = (from.CellPosition.Column + to.CellPosition.Column) / 2;
            return _squares[row][col];
        }

        private void IsPromotionMove(Cell cell)
        {
            if (cell.Piece.IsKing)
            {
                return;
            }
            if (cell.Piece.Color == Player.White && cell.CellPosition.Row == 7)
            {
                cell.Piece.IsKing = true;
                cell.ImagePath = "../../Resources/whiteKing.png";
            }
            else if (cell.Piece.Color == Player.Red && cell.CellPosition.Row == 0)
            {
                cell.Piece.IsKing = true;
                cell.ImagePath = "../../Resources/redKing.png";
            }
        }
        
    }
}
