namespace Checkers.Events;
public class MoveMadeEventArgs
{
    public int CheckerInitialIndex;
    public int CheckerNewIndex;

    public MoveMadeEventArgs(int checkerInitialIndex, int checkerNewIndex)
    {
        CheckerInitialIndex = checkerInitialIndex;
        CheckerNewIndex = checkerNewIndex;
    }
}
