using Caliburn.Micro;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using Checkers.MVVM.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;

namespace Checkers.Models
{
    public class Board
    {
        //private readonly Piece[,] pieces= new Piece[8, 8];

        //public Piece this[int row, int col]
        //{
        //    get
        //    {
        //        return pieces[row, col];
        //    }
        //    set
        //    {
        //        pieces[row, col] = value;
        //    }
        //}
        //public Piece this[Position position]
        //{
        //    get
        //    {
        //        return this[position.Row, position.Column];
        //    }
        //    set
        //    {
        //        this[position.Row, position.Column] = value;
        //    }
        //}
        //public static Board Initial()
        //{
        //    Board board = new Board();
        //    board.AddStartPieces();
        //    return board;
        //}

        //private void AddStartPieces()
        //{
        //    this[0,1]= new Piece(Player.White);
        //    this[0,3]= new Piece(Player.White);
        //    this[0,5]= new Piece(Player.White);
        //    this[0,7]= new Piece(Player.White);
        //    this[1,0]= new Piece(Player.White);
        //    this[1,2]= new Piece(Player.White);
        //    this[1,4]= new Piece(Player.White);
        //    this[1,6]= new Piece(Player.White);
        //    this[2,1]= new Piece(Player.White);
        //    this[2,3]= new Piece(Player.White);
        //    this[2,5]= new Piece(Player.White);
        //    this[2,7]= new Piece(Player.White);

        //    this[5,0]= new Piece(Player.Red);
        //    this[5,2]= new Piece(Player.Red);
        //    this[5,4]= new Piece(Player.Red);
        //    this[5,6]= new Piece(Player.Red);
        //    this[6,1]= new Piece(Player.Red);
        //    this[6,3]= new Piece(Player.Red);
        //    this[6,5]= new Piece(Player.Red);
        //    this[6,7]= new Piece(Player.Red);
        //    this[7,0]= new Piece(Player.Red);
        //    this[7,2]= new Piece(Player.Red);
        //    this[7,4]= new Piece(Player.Red);
        //    this[7,6]= new Piece(Player.Red);

        //}
        public static bool IsInsideBoard(Position position)
        {
            return position.Row >= 0 && position.Row < 8 && position.Column >= 0 && position.Column < 8;
        }
        public static bool IsEmpty(Position position, BindableCollection<BindableCollection<Cell>> cells)
        {
            foreach (var row in cells)
            {
                foreach (var cell in row)
                {
                    if (cell.CellPosition == position)
                    {
                        return cell.ImagePath == "../../Resources/transparent.png" || 
                            cell.ImagePath== "../../Resources/Green.png";
                    }
                }
            }
            return false;
        }

        public static Piece GetPiece(Position position, BindableCollection<BindableCollection<Cell>> Squares)
        {
            foreach (var row in Squares)
            {
                foreach (var cell in row)
                {
                    if (cell.CellPosition == position)
                    {
                        return cell.Piece;
                    }
                }
            }
            return null;
        }

        public static Piece GetPiece(int row, int col, BindableCollection<BindableCollection<Cell>> Squares)
        {
            return GetPiece(new Position(row, col), Squares);
        }

        public static BindableCollection<BindableCollection<Cell>> GetInitialCells()
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

        public static void SetPieceToPosition(Position toPos, Piece piece, BindableCollection<BindableCollection<Cell>> cells)
        {
            foreach (var row in cells)
            {
                foreach (var cell in row)
                {
                    if (cell.CellPosition == toPos)
                    {
                        cell.Piece = piece;
                        if (piece != null)
                        {
                            cell.ImagePath = piece.Color == Player.White ? "../../Resources/whitePiece.png" : "../../Resources/redPiece.png";
                        }
                        else
                        {
                            cell.ImagePath = "../../Resources/transparent.png";
                        }
                    }
                }
            }
        }

        public static void SaveGame(BindableCollection<BindableCollection<Cell>> cells)
        {
            string filePath= "../../JSONs/game.json";
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
            else if(result==MessageBoxResult.No) filePath = "../../JSONs/game.json";
            else if (result == MessageBoxResult.Cancel) return;
            
            BoardViewModel.DeleteGreen();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            List<string> jsonList = new List<string>();

            string gameOverJson;
            // Serialize non-cell variables

            if (BoardViewModel.GameOver == true)
            {
                gameOverJson = Newtonsoft.Json.JsonConvert.SerializeObject(1);
            }
            else gameOverJson = Newtonsoft.Json.JsonConvert.SerializeObject(0);

            string currentPlayerJson = Newtonsoft.Json.JsonConvert.SerializeObject(BoardViewModel.CurrentPlayer);
            jsonList.Add(gameOverJson);
            jsonList.Add(currentPlayerJson);

            // Serialize cell data
            foreach (var row in cells)
            {
                foreach (var cell in row)
                {
                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cell);
                    jsonList.Add(jsonData);
                }
            }

            // Convert the list of JSON strings into a single JSON array string
            string jsonArray = "[" + string.Join(",", jsonList) + "]";

            // Write the JSON array to the file
            File.WriteAllText(filePath, jsonArray);
        }


        public static (int gameState,int currentPlayer, BindableCollection<BindableCollection<Cell>> cells) ReadFromJson()
        {
            string filePath = "../../JSONs/game.json";
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to load game from a specific location?", "Load Game",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                
                    Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                    dialog.Filter = "JSON files (*.json)|*.json";
                    dialog.DefaultExt = ".json";
                    string relativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSONs");
                    dialog.InitialDirectory = relativePath;
                    dialog.Title = "Select a JSON file";
                    bool? dialogResult = dialog.ShowDialog();
                    
                    if (dialogResult == true)
                    {
                        filePath = dialog.FileName;
                    }
                    else return (-1,-1,new BindableCollection<BindableCollection<Cell>>());
                
            }
            else if (result == MessageBoxResult.No) filePath = "../../JSONs/game.json";
            else if (result == MessageBoxResult.Cancel) return (-1, -1, new BindableCollection<BindableCollection<Cell>>());
            // Read the entire content of the file as a single string
            string jsonData = File.ReadAllText(filePath);

            // Deserialize the JSON string into a JArray
            JArray jsonArray = JArray.Parse(jsonData);

            // Extract the non-cell variables from the first two elements of the JSON array
            int gameState = jsonArray[0].ToObject<int>();
            int currentPlayer = jsonArray[1].ToObject<int>();

            // Extract the cell data from the remaining elements of the JSON array
            JArray cellArray = new JArray(jsonArray.Skip(2));
            BindableCollection<BindableCollection<Cell>> cells = new BindableCollection<BindableCollection<Cell>>();

            // Initialize the outer collection
            for (int i = 0; i < 8; i++)
            {
                cells.Add(new BindableCollection<Cell>());
            }

            foreach (JToken cellToken in cellArray)
            {
                // Deserialize CellPosition
                Position cellPosition = cellToken["CellPosition"].ToObject<Position>();

                // Deserialize Piece
                Piece piece = cellToken["Piece"]?.ToObject<Piece>();

                // Read ImagePath
                string imagePath = (string)cellToken["ImagePath"];

                // Create Cell object
                Cell cell = new Cell
                {
                    CellPosition = cellPosition,
                    Piece = piece,
                    ImagePath = imagePath
                };

                // Add the cell to the appropriate row
                cells[cellPosition.Row].Add(cell);
            }

            return (gameState,currentPlayer, cells);
        }
    }
}


