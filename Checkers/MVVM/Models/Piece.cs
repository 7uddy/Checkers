using Caliburn.Micro;
using Checkers.MVVM.Logic;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using System.Collections;
using System.Collections.Generic;

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
        public Piece Copy()
        {
            Piece piece = new Piece(_color,_isKing);
            return piece;
        }

        public Position GetMoves(Position from, BindableCollection<BindableCollection<Cell>> cells, Direction dir)
        {

            Position pos = dir.NewPosition(from);

            if (Board.IsEmpty(pos, cells))
            {
                return pos;
            }
            else
            {
                if (Board.IsInsideBoard(pos) && Board.GetPiece(pos, cells).Color != _color)
                {
                    pos = dir.NewPosition(pos);
                    if (Board.IsEmpty(pos, cells))
                    {
                        return pos;
                    }
                }
            }

            return null;
        }
    }
}
