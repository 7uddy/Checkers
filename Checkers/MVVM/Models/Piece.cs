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

        public Position GetMoves(Position from, BindableCollection<BindableCollection<Cell>> cells,Direction dir)
        {

            if(!_isKing)
            {
                Position pos=new Position();
                if(_color==Player.White)
                {
                    if(dir==Direction.SouthEast)
                    {
                        pos = Direction.SouthEast.NewPosition(from);
                    }
                    else if(dir==Direction.SouthWest)
                    {
                        pos = Direction.SouthWest.NewPosition(from);
                    }
                }
                else
                {
                    if(dir==Direction.NorthWest)
                    {
                        pos = Direction.NorthWest.NewPosition(from);
                    }
                    else if(dir==Direction.NorthEast)
                    {
                        pos = Direction.NorthEast.NewPosition(from);
                    }
                }

                if (Board.IsEmpty(pos, cells) && Board.GetPiece(pos, cells) == null)
                {
                    return pos;
                }
            }
            return null;
        }
    }
}
