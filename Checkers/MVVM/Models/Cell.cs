using Checkers.Models;
using Checkers.MVVM.Services;

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
