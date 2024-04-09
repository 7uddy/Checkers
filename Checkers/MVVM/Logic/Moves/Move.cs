using Caliburn.Micro;
using Checkers.MVVM.Models;
using Checkers.MVVM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.MVVM.Logic
{
    public abstract class Move
    {
        public abstract MoveType MoveType { get; }

        public abstract Position FromPos { get; }
        public abstract Position ToPos { get; }
        public abstract void Execute(BindableCollection<BindableCollection<Cell>> cells);
    }
}
