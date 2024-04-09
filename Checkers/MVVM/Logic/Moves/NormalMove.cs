using Caliburn.Micro;
using Checkers.Models;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.MVVM.Logic.Moves
{
    public class NormalMove : Move
    {
        public override MoveType MoveType => MoveType.Normal;

        public override Position FromPos { get; }

        public override Position ToPos { get; }

        public NormalMove(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
        }

        public override void Execute(BindableCollection<BindableCollection<Cell>> cells)
        {
            Piece piece=Board.GetPiece(FromPos,cells);
            Board.SetPieceToPosition(ToPos, piece, cells);
            Board.SetPieceToPosition(FromPos, null, cells);
        }
    }
}
