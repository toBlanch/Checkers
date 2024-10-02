namespace Checkers.Behaviours;

public class UpdateCheckerColorEventArgs(char item, int index)
{
    public char checkerType = item;
    public int index = index;
}
