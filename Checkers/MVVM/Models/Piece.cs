using Checkers.MVVM.Services;

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
        public Piece(Player color)
        {
            _color = color;
        }
        public Piece Copy()
        {
            Piece piece = new Piece(_color);
            piece.IsKing = _isKing;
            return piece;
        }
    }
}
