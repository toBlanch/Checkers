namespace Checkers.Model;
internal class CheckersBoard
{
    public char[] BoardTiles = new char[64];
    public int SelectedTileIndex = -1;
    public bool RedPlayerTurn = true;
    public bool CurrentPlayerCanTakePiece = false;
    public int ConnectedPlayers = -1;
    public int ID = -1;
}