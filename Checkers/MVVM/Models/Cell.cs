using Checkers.Models;
using Checkers.MVVM.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.MVVM.Models
{
    public class Cell
    {
        public Position CellPosition { get; set; }
        public Piece Piece { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
    }
}
