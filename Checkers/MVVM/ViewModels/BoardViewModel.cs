using Caliburn.Micro;
using Checkers.Commands;
using Checkers.Models;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using Checkers.Stores;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Checkers.MVVM.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public static Player CurrentPlayer { get; private set; }

        public static bool GameOver { get; private set; }

        private static BindableCollection<BindableCollection<Cell>> _squares;

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
            _squares = GetInitialCells();
            AboutViewModel.ReadWins();
            SettingsViewModel.ReadSettings();
            CheckGame();
        }
        private BindableCollection<BindableCollection<Cell>> GetInitialCells()
        {
            BindableCollection<BindableCollection<Cell>> cells = new BindableCollection<BindableCollection<Cell>>();

            //White pieces
            for (int row = 0; row < 3; row++)
            {
                BindableCollection<Cell> rowCells = new BindableCollection<Cell>();
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 1)
                        rowCells.Add(new Cell { CellPosition = new Position(row, col), Piece = new Piece(Player.White, false), ImagePath = "../../Resources/whitePiece.png" });
                    else
                        rowCells.Add(new Cell { CellPosition = new Position(row, col), Piece = null, ImagePath = "../../Resources/transparent.png" });
                }
                cells.Add(rowCells);
            }

            //Empty cells
            for (int row = 3; row < 5; row++)
            {
                BindableCollection<Cell> rowCells = new BindableCollection<Cell>();
                for (int col = 0; col < 8; col++)
                {
                    rowCells.Add(new Cell { CellPosition = new Position(row, col), Piece = null, ImagePath = "../../Resources/transparent.png" });
                }
                cells.Add(rowCells);
            }

            //Red pieces
            for (int row = 5; row < 8; row++)
            {
                BindableCollection<Cell> rowCells = new BindableCollection<Cell>();
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 1)
                        rowCells.Add(new Cell { CellPosition = new Position(row, col), Piece = new Piece(Player.Red, false), ImagePath = "../../Resources/redPiece.png" });
                    else
                        rowCells.Add(new Cell { CellPosition = new Position(row, col), Piece = null, ImagePath = "../../Resources/transparent.png" });
                }
                cells.Add(rowCells);
            }

            return cells;
        }
        private Cell SimpleCell { get; set; }
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

        private void OnCellClicked(Cell clickedCell)
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
                string jsonText = JsonConvert.SerializeObject(integers);

                File.WriteAllText(filePath, jsonText);
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

        private void DeleteGreen()
        {
            if (GreenCells != null)
            {
                foreach (var cell in GreenCells)
                {
                    if (cell.Piece == null) cell.ImagePath = "../../Resources/transparent.png";
                }
            }
        }
        private void SetAcceptedMoves(Cell clickedCell)
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
        public ICommand SaveGameCommand
        {
            get
            {
                if (_saveGame == null)
                {
                    _saveGame = new RelayCommand(() => { 
                        DeleteGreen(); 
                        Squares = new BindableCollection<BindableCollection<Cell>>(_squares);
                        SaveGame(); 
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
                        var (gameState, currentPlayer, cells) = ReadFromJson();

                        if (gameState == -1) return;

                        _squares = cells;
                        Squares = new BindableCollection<BindableCollection<Cell>>(_squares);

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
                    MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to start a new game?", 
                        "New Game",MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        CurrentPlayer = Player.Red;
                        GameOver = false;
                        _squares = GetInitialCells();
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
        public bool SaveGameButton { 
            get {  return !GameOver; } 
        }
        public static bool IsInsideBoard(Position position)
        {
            return position.Row >= 0 && position.Row < 8 && position.Column >= 0 && position.Column < 8;
        }
        public static bool IsEmpty(Position position)
        {
            foreach (var row in _squares)
            {
                foreach (var cell in row)
                {
                    if (cell.CellPosition == position)
                    {
                        return cell.ImagePath == "../../Resources/transparent.png" ||
                            cell.ImagePath == "../../Resources/Green.png";
                    }
                }
            }
            return false;
        }

        private string filePath = "../../JSONs/game.json";
        private void SaveGame()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to save game to a specific location?", "Save Game",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult dialogResult = dialog.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        filePath = dialog.SelectedPath + "\\game.json";
                    }
                    else return;
                }
            }
            else if (result == MessageBoxResult.No) filePath = "../../JSONs/game.json";
            else if (result == MessageBoxResult.Cancel) return;

            DeleteGreen();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            List<string> jsonList = new List<string>();

            string gameOverJson;

            if (GameOver == true)
            {
                gameOverJson = JsonConvert.SerializeObject(1);
            }
            else gameOverJson = JsonConvert.SerializeObject(0);

            string currentPlayerJson = JsonConvert.SerializeObject(CurrentPlayer);
            jsonList.Add(gameOverJson);
            jsonList.Add(currentPlayerJson);

            foreach (var row in _squares)
            {
                foreach (var cell in row)
                {
                    string jsonData = JsonConvert.SerializeObject(cell);
                    jsonList.Add(jsonData);
                }
            }

            string jsonArray = "[" + string.Join(",", jsonList) + "]";

            File.WriteAllText(filePath, jsonArray);
        }
        private (int gameState, int currentPlayer, BindableCollection<BindableCollection<Cell>> cells) ReadFromJson()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to load game from a specific location?", "Load Game",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {

                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.DefaultExt = ".json";
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string parentDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent?.FullName;
                string relativePath = Path.Combine(parentDirectory, "JSONs");
                dialog.InitialDirectory = relativePath;
                dialog.Title = "Select a JSON file";
                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult == true)
                {
                    filePath = dialog.FileName;
                }
                else return (-1, -1, new BindableCollection<BindableCollection<Cell>>());

            }
            else if (result == MessageBoxResult.No) filePath = "../../JSONs/game.json";
            else if (result == MessageBoxResult.Cancel) return (-1, -1, new BindableCollection<BindableCollection<Cell>>());

            string jsonData = File.ReadAllText(filePath);


            JArray jsonArray = JArray.Parse(jsonData);


            int gameState = jsonArray[0].ToObject<int>();
            int currentPlayer = jsonArray[1].ToObject<int>();


            JArray cellArray = new JArray(jsonArray.Skip(2));
            BindableCollection<BindableCollection<Cell>> cells = new BindableCollection<BindableCollection<Cell>>();

            for (int i = 0; i < 8; i++)
            {
                cells.Add(new BindableCollection<Cell>());
            }

            foreach (JToken cellToken in cellArray)
            {
                Position cellPosition = cellToken["CellPosition"].ToObject<Position>();

                Piece piece = cellToken["Piece"]?.ToObject<Piece>();

                string imagePath = (string)cellToken["ImagePath"];

                Cell cell = new Cell
                {
                    CellPosition = cellPosition,
                    Piece = piece,
                    ImagePath = imagePath
                };

                cells[cellPosition.Row].Add(cell);
            }

            return (gameState, currentPlayer, cells);
        }
    }
}
