namespace Checkers.Model;
internal class Main
{
    public char[] Board = new char[64];
    public int SelectedTileIndex = -1;
    public bool RedPlayerTurn = true;
    public bool CurrentPlayerCanTakePiece = false;
}
