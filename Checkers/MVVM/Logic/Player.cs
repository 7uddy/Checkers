namespace Checkers.MVVM.Services
{
    public enum Player
    {
        None,
        White,
        Red
    }

    public static class PlayerExtensions
    {
        public static Player Opponent(this Player player)
        {
            switch (player)
            {
                case Player.White:
                    return Player.Red;
                case Player.Red:
                    return Player.White;
                default:
                    return Player.None;
            }
        }
    }
}
