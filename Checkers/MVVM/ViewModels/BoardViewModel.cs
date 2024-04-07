using Caliburn.Micro;
using Checkers.Models;
using Checkers.MVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }

        private readonly BindableCollection<BindableCollection<Image>> _squares;

        public BindableCollection<BindableCollection<Image>> Squares
        {
            get { return _squares; }
        }

        public BoardViewModel()
        {
            Board= Board.Initial();
            _squares = new BindableCollection<BindableCollection<Image>>();
            InitializeBoard(Board);
        }

        public BoardViewModel(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
            InitializeBoard(Board);
        }

        private void InitializeBoard(Board board)
        {
            if (board is null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            for (int row = 0; row < 8; row++)
            {
                var squareRow = new BindableCollection<Image>();
                for(int col = 0; col < 8; col++)
                {
                    Piece piece= board[row, col];
                    var square = new Image();
                    if(piece==null)
                    {
                        square.Source = new ImageSourceConverter().ConvertFromString("../../Resources/transparent.png") as ImageSource;
                        squareRow.Add(square);
                        continue;
                    }
                    if(piece.Color==Player.Red)
                    {
                        square.Source = new ImageSourceConverter().ConvertFromString("../../Resources/blackpiece.png") as ImageSource;
                    }
                    else
                    {
                        square.Source = new ImageSourceConverter().ConvertFromString("../../Resources/whitepiece.png") as ImageSource;
                    }
                    squareRow.Add(square);
                }
                _squares.Add(squareRow);
            }
            
        }
    }
}
