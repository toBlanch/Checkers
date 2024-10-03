namespace Checkers.Events;

public class UpdateCheckerColorEventArgs(char item, int index)
{
    public char checkerType = item;
    public int index = index;
}
