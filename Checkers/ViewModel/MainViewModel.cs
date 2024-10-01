namespace Checkers.ViewModel;

class MainViewModel
{
    public char[] Board { get; } = new char[64];
    public MainViewModel()
    {
        for (int i = 0; i < 64; i++)
        {
            bool isOnBlackTile = i % 2 == i / 8 % 2;
            bool isOnTopThreeRows = i < 24;
            Board[i] = isOnBlackTile
                ? ' '
                : isOnTopThreeRows
                    ? 'b'
                    : i > 39
                        ? 'r'
                        : ' ';
        }
    }
}
