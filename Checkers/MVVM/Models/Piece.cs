using Caliburn.Micro;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using Checkers.MVVM.ViewModels;

namespace Checkers.Models
{
    public class Piece
    {
        private Player _color;
        public Player Color
        {
            get { return _color; }
            set { _color = value; }
        }
        private bool _isKing;
        public bool IsKing
        {
            get { return _isKing; }
            set { _isKing = value; }
        }
        public Piece(Player color,bool isKing)
        {
            _color = color;
            _isKing = isKing;
        }
        public Position GetMoves(Position from, BindableCollection<BindableCollection<Cell>> cells, Direction dir)
        {

            Position pos = dir.NewPosition(from);

            if (BoardViewModel.IsEmpty(pos))
            {
                return pos;
            }
            else
            {
                if (BoardViewModel.IsInsideBoard(pos) && pos.GetPiece(cells).Color != _color)
                {
                    pos = dir.NewPosition(pos);
                    if (BoardViewModel.IsEmpty(pos))
                    {
                        return pos;
                    }
                }
            }
            return null;
        }
    }
}
