namespace Checkers.MVVM.Services
{
    public class Direction
    {
        public readonly static Direction NorthWest= new Direction(-1, -1);
        public readonly static Direction NorthEast = new Direction(-1, 1);
        public readonly static Direction SouthWest = new Direction(1, -1);
        public readonly static Direction SouthEast = new Direction(1, 1);
        public int RowDelta { get;}
        public int ColumnDelta { get; }
        public Direction(int rowDelta, int columnDelta)
        {
            RowDelta = rowDelta;
            ColumnDelta = columnDelta;
        }
        public static Direction operator +(Direction a, Direction b)
        {
            return new Direction(a.RowDelta + b.RowDelta, a.ColumnDelta + b.ColumnDelta);
        }

        public static Direction operator * (int scalar, Direction direction)
        {
            return new Direction(scalar * direction.RowDelta, scalar * direction.ColumnDelta);
        }

        public Position NewPosition(Position position)
        {
            return new Position(position.Row + RowDelta, position.Column + ColumnDelta);
        }
    }
}
