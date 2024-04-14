using Caliburn.Micro;
using Checkers.Commands;
using Checkers.Models;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using Checkers.Stores;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
            NavigateToMenu = new NavigateCommand(navigation, createMenuViewModel);
            CurrentPlayer = Player.Red;
            GameOver = false;
            _squares = Board.GetInitialCells();
            ReadWins();
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
                    if (SettingsViewModel.IsMultiJumpToggled)
                    {
                        clickedCell = Multijump(clickedCell);
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
            bool noAvailableMoves = true;
            bool gameOver = true;
            int whitePieces = 0;
            int redPieces = 0;
            foreach (var row in _squares)
            {
                foreach (var cell in row)
                {
                    if (cell.Piece != null && cell.Piece.Color == CurrentPlayer)
                    {
                        gameOver = false;

                        if (cell.Piece.Color == Player.White && cell.Piece.IsKing == false &&
                            (cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthEast) != null ||
                             cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthWest) != null))
                        {
                            cell.ImagePath = "../../Resources/availableWhitePiece.png";
                            noAvailableMoves = false;
                        }

                        else if (cell.Piece.Color == Player.Red && cell.Piece.IsKing == false && (
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthWest) != null))
                        {
                            cell.ImagePath = "../../Resources/availableRedPiece.png";
                            noAvailableMoves = false;
                        }

                        else if (cell.Piece.Color == Player.White && cell.Piece.IsKing == true &&
                            (cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthWest) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthWest) != null))
                        {
                            cell.ImagePath = "../../Resources/availableWhiteKing.png";
                            noAvailableMoves = false;
                        }

                        else if (cell.Piece.Color == Player.Red && cell.Piece.IsKing == true &&
                            (cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.NorthWest) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthEast) != null ||
                            cell.Piece.GetMoves(cell.CellPosition, _squares, Direction.SouthWest) != null))
                        {
                            cell.ImagePath = "../../Resources/availableRedKing.png";
                            noAvailableMoves = false;
                        }
                    }
                    else if (cell.Piece != null && cell.Piece.Color != CurrentPlayer)
                    {
                        if (cell.Piece.Color == Player.White && cell.Piece.IsKing == false)
                            cell.ImagePath = "../../Resources/whitePiece.png";
                        else if (cell.Piece.Color == Player.Red && cell.Piece.IsKing == false)
                            cell.ImagePath = "../../Resources/redPiece.png";
                        else if (cell.Piece.Color == Player.White && cell.Piece.IsKing == true)
                            cell.ImagePath = "../../Resources/whiteKing.png";
                        else if (cell.Piece.Color == Player.Red && cell.Piece.IsKing == true)
                            cell.ImagePath = "../../Resources/redKing.png";
                    }
                    if (cell.Piece != null && cell.Piece.Color == Player.White)
                    {
                        whitePieces++;
                    }
                    else if (cell.Piece != null && cell.Piece.Color == Player.Red)
                    {
                        redPieces++;
                    }
                }
            }
            if (gameOver || noAvailableMoves)
            {
                GameOver = true;
                if (CurrentPlayer == Player.White)
                {
                    AboutViewModel.RedWins++;
                }
                else
                {
                    AboutViewModel.WhiteWins++;
                }
                if(whitePieces> AboutViewModel.MaximumWhitePieces)
                {
                    AboutViewModel.MaximumWhitePieces = whitePieces;
                }
                if (redPieces > AboutViewModel.MaximumRedPieces)
                {
                    AboutViewModel.MaximumRedPieces = redPieces;
                }
                WriteWins();
                OnPropertyChanged(nameof(GameOver));
                OnPropertyChanged(nameof(RedWonImage));
                OnPropertyChanged(nameof(WhiteWonImage));
                OnPropertyChanged(nameof(SaveGameButton));
            }
        }
        private void WriteWins()
        {
            string filePath = "../../JSONs/stats.json";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            int[] integers = { AboutViewModel.WhiteWins, AboutViewModel.MaximumWhitePieces, 
                AboutViewModel.RedWins, AboutViewModel.MaximumRedPieces };
            try
            {
                // Serialize the integers array to JSON format
                string jsonText = JsonConvert.SerializeObject(integers);

                // Write the JSON text to the file
                File.WriteAllText(filePath, jsonText);

                Console.WriteLine("Successfully wrote 4 integers to the JSON file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public static void ReadWins()
        {
            string filePath = "../../JSONs/stats.json";
            if (!File.Exists(filePath))
            {
                AboutViewModel.WhiteWins = 0;
                AboutViewModel.MaximumWhitePieces = 0;
                AboutViewModel.RedWins = 0;
                AboutViewModel.MaximumRedPieces = 0;
                return;
            }
            string jsonText = File.ReadAllText(filePath);
            try
            {
                // Deserialize the JSON text into an array of integers
                int[] integers = JsonConvert.DeserializeObject<int[]>(jsonText);

                // Check if there are exactly 4 integers
                if (integers.Length == 4)
                {
                    // Print the integers
                    Console.WriteLine("The 4 integers read from the JSON file are:");
                    AboutViewModel.WhiteWins= integers[0];
                    AboutViewModel.MaximumWhitePieces = integers[1];
                    AboutViewModel.RedWins = integers[2];
                    AboutViewModel.MaximumRedPieces = integers[3];
                }
                else
                {
                    Console.WriteLine("The JSON file does not contain exactly 4 integers.");
                }
            }
            catch (JsonException)
            {
                Console.WriteLine("Error deserializing JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
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
                        CheckGame();
                        OnPropertyChanged(nameof(GameOver));
                        OnPropertyChanged(nameof(RedWonImage));
                        OnPropertyChanged(nameof(WhiteWonImage));
                        OnPropertyChanged(nameof(SaveGameButton));
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
                        CheckGame();
                        Squares = new BindableCollection<BindableCollection<Cell>>(_squares);
                        OnPropertyChanged(nameof(GameOver));
                        OnPropertyChanged(nameof(RedWonImage));
                        OnPropertyChanged(nameof(WhiteWonImage));
                        OnPropertyChanged(nameof(SaveGameButton));
                    }
                    else return;
                });
            }
        }

        public System.Windows.Visibility RedWonImage
        {
            get
            {
                if(GameOver && CurrentPlayer==Player.White)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }
        public System.Windows.Visibility WhiteWonImage
        {
            get
            {
                if(GameOver && CurrentPlayer==Player.Red)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }
        public bool SaveGameButton { get {  return !GameOver; } }
    }
}
