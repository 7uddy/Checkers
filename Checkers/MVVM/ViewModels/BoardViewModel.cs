using Caliburn.Micro;
using Checkers.Commands;
using Checkers.Models;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using Checkers.Stores;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Checkers.MVVM.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public static Player CurrentPlayer { get; private set; }

        public static bool GameOver { get; private set; }

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
        public ICommand NavigateToMenu { get; }
        public BoardViewModel(Navigation navigation, Func<MenuViewModel> createMenuViewModel)
        {
            NavigateToMenu=new NavigateCommand(navigation, createMenuViewModel);
            CurrentPlayer = Player.Red;
            GameOver = false;
            _squares = Board.GetInitialCells();
            CheckGame();
        }
        public Cell SimpleCell { get; set; }
        static private List<Cell> GreenCells { get; set; }

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
            if (clickedCell.ImagePath == "../../Resources/Green.png")
            {
                MoveToNewCell(clickedCell);

                if (IsCaptureMove(SimpleCell, clickedCell))
                {
                    Cell capturedCell = GetCapturedCell(SimpleCell, clickedCell);
                    capturedCell.Piece = null;
                    capturedCell.ImagePath = "../../Resources/transparent.png";
                    if(SettingsViewModel.IsMultiJumpToggled)
                    {
                        clickedCell=Multijump(clickedCell);
                    }
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
            CheckGame();
            Squares = new BindableCollection<BindableCollection<Cell>>(_squares);

        }

        private void CheckGame()
        {
            bool gameOver = true;
            foreach (var row in _squares)
            {
                foreach (var cell in row)
                {
                    if (cell.Piece != null && cell.Piece.Color == CurrentPlayer)
                    {
                        gameOver = false;
                        if (cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthWest) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthWest) != null)
                        {
                            if (cell.Piece.Color == Player.White && cell.Piece.IsKing == false)
                                cell.ImagePath = "../../Resources/availableWhitePiece.png";
                            else if (cell.Piece.Color == Player.Red && cell.Piece.IsKing == false)
                                cell.ImagePath = "../../Resources/availableRedPiece.png";
                            else if (cell.Piece.Color == Player.White && cell.Piece.IsKing == true)
                                cell.ImagePath = "../../Resources/availableWhiteKing.png";
                            else if (cell.Piece.Color == Player.Red && cell.Piece.IsKing == true)
                                cell.ImagePath = "../../Resources/availableRedKing.png";
                        }
                    }
                    else if (cell.Piece != null && cell.Piece.Color != CurrentPlayer)
                    {
                        if(cell.Piece.Color==Player.White && cell.Piece.IsKing==false)
                            cell.ImagePath = "../../Resources/whitePiece.png";
                        else if(cell.Piece.Color==Player.Red && cell.Piece.IsKing==false)
                            cell.ImagePath = "../../Resources/redPiece.png";
                        else if(cell.Piece.Color==Player.White && cell.Piece.IsKing==true)
                            cell.ImagePath = "../../Resources/whiteKing.png";
                        else if(cell.Piece.Color==Player.Red && cell.Piece.IsKing==true)
                            cell.ImagePath = "../../Resources/redKing.png";
                    }
                }
            }
            GameOver = gameOver;
        }

        private Cell Multijump(Cell clickedCell)
        {
            DeleteGreen();
            IsPromotionMove(clickedCell);
            List<Position> positions = new List<Position>();

            if (clickedCell.Piece.Color == Player.White)
            {
                positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthWest));
                positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthEast));
                if (clickedCell.Piece.IsKing == true)
                {
                    positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthWest));
                    positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthEast));
                }
            }
            else
            {
                positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthWest));
                positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.NorthEast));
                if (clickedCell.Piece.IsKing == true)
                {
                    positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthWest));
                    positions.Add(clickedCell.Piece.GetMoves(clickedCell.CellPosition, _squares, Direction.SouthEast));
                }
            }

            Utils.Shuffle(positions);

            foreach (var pos in positions)
            {
                if (pos == null) continue;

                if (IsCaptureMove(clickedCell, _squares[pos.Row][pos.Column]))
                {
                    SimpleCell = clickedCell;
                    clickedCell = _squares[pos.Row][pos.Column];
                    MoveToNewCell(clickedCell);
                    Cell capturedCell = GetCapturedCell(SimpleCell, clickedCell);
                    capturedCell.Piece = null;
                    capturedCell.ImagePath = "../../Resources/transparent.png";
                    clickedCell=Multijump(clickedCell);
                    break;
                }
            }
            return clickedCell;

        }

        private void MoveToNewCell(Cell clickedCell)
        {

            clickedCell.Piece = SimpleCell.Piece;
            clickedCell.ImagePath = SimpleCell.ImagePath;
            SimpleCell.Piece = null;
            SimpleCell.ImagePath = "../../Resources/transparent.png";

        }

        public static void DeleteGreen()
        {
            if (GreenCells != null)
            {
                foreach (var cell in GreenCells)
                {
                    if (cell.Piece == null) cell.ImagePath = "../../Resources/transparent.png";
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
        private bool IsCaptureMove(Cell from, Cell to)
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

        private ICommand _saveGame;
        public ICommand SaveGame
        {
            get
            {
                if (_saveGame == null)
                {
                    _saveGame = new RelayCommand(() => { 
                        DeleteGreen(); 
                        Squares = new BindableCollection<BindableCollection<Cell>>(_squares);
                        Board.SaveGame(_squares); 
                    });
                }
                return _saveGame;
            }
        }

        private ICommand _loadGame;
        public ICommand LoadGame
        {
            get
            {
                if (_loadGame == null)
                {
                    _loadGame = new RelayCommand(() =>
                    {

                        // Read data from JSON file
                        var (gameState, currentPlayer, cells) = Board.ReadFromJson();

                        if (gameState == -1) return;
                        // Process the loaded data as needed
                        // For example, update your ViewModel properties with the loaded data
                        _squares = cells;
                        Squares = new BindableCollection<BindableCollection<Cell>>(_squares);

                        // Once the data is loaded, you can update your ViewModel properties
                        // or trigger other actions as needed
                        if (gameState == 1) {
                            GameOver = true;
                        }
                        else GameOver = false;

                        switch (currentPlayer)
                        {
                            case 1:
                                CurrentPlayer = Player.White;
                                break;

                            case 2:
                                CurrentPlayer = Player.Red;
                                break;

                            default:
                                CurrentPlayer = Player.None;
                                break;
                        }
                    });
                }
                return _loadGame;
            }
        }

        public ICommand NewGame
        {
            get
            {
                return new RelayCommand(() =>
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to start a new game?", "New Game",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        CurrentPlayer = Player.Red;
                        GameOver = false;
                        _squares = Board.GetInitialCells();
                        Squares = new BindableCollection<BindableCollection<Cell>>(_squares);
                    }
                    else return;
                });
            }
        }
    }
}
