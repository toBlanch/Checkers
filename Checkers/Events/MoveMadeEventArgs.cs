namespace Checkers.Events;
public class MoveMadeEventArgs(int checkerInitialIndex, int checkerNewIndex)
{
    public int CheckerInitialIndex = checkerInitialIndex;
    public int CheckerNewIndex = checkerNewIndex;
}
