using CanvasRect;
using Checkers.Behaviours;
using Checkers.Events;
using Checkers.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Checkers.ViewModel;
internal class CheckersBoardViewModel : CanvasRectBase
{
    private CheckersBoard _board = new();

    public static event ColorChangedEventhandler? ColorChanged;
    public char[] BoardTiles
    {
        get => _board.BoardTiles;
        set => _board.BoardTiles = value;
    }
    public int SelectedTileIndex
    {
        get => _board.SelectedTileIndex;
        set => _board.SelectedTileIndex = value;
    }
    public bool RedPlayerTurn
    {
        get => _board.RedPlayerTurn;
        set => _board.RedPlayerTurn = value;
    }
    public bool CurrentPlayerCanTakePiece
    {
        get => _board.CurrentPlayerCanTakePiece;
        set => _board.CurrentPlayerCanTakePiece = value;
    }
    char IntendedCheckerType => RedPlayerTurn
                                ? 'r'
                                : 'b';
    char OpposingCheckerType => RedPlayerTurn
                                ? 'b'
                                : 'r';
    public int ConnectedPlayers
    {
        get => _board.ConnectedPlayers;
        set
        {
            _board.ConnectedPlayers = value;
            RaisePropertyChanged(); //TEMPORARY
        }
    }
    public int ID
    {
        get => _board.ID;
        set => _board.ID = value;
    }

    public CheckersBoardViewModel()
    {
        Width = 800;
        Height = 600;

        for (int i = 0; i < 64; i++)
        {
            bool isOnBlackTile = i % 2 == i / 8 % 2;
            bool isOnTopThreeRows = i < 24;
            UpdateBoardTile(i, isOnBlackTile
                ? ' '
                : isOnTopThreeRows
                    ? 'b'
                    : i > 39
                        ? 'r'
                        : ' ');
        }
    }

    public void MoveMade(MoveMadeEventArgs e)
    {
        SelectedTileIndex = e.CheckerInitialIndex;
    }

    public void UpdateConnectedPlayers(int e)
    {
        ConnectedPlayers = e;
        if (ID == -1)
        {
            ID = ConnectedPlayers - 1;
        }
    }

    internal void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Grid grid = (Grid)sender;
        DependencyObject dataTemplate = VisualTreeHelper.GetParent(grid);
        DependencyObject uniformGrid = VisualTreeHelper.GetParent(dataTemplate);

        if (uniformGrid is UniformGrid parentGrid)
        {
            int index = parentGrid.Children.IndexOf((UIElement)dataTemplate);
            TileClicked(index);
        }
    }
    private void TileClicked(int index)
    {
        switch (char.ToLower(BoardTiles[index]))
        {
            case 'b':
            case 'r':
                RemoveHighlightedTiles();
                if (PlayerCanMovePiece())
                {
                    HighlightPossibleMoves(index);
                }
                break;
            case 'g':
                MovePiece(index);
                RemoveHighlightedTiles();
                if (CurrentPlayerCanTakePiece && CanCheckerTakeAnyPiece(index))
                {
                    SelectedTileIndex = index;
                    HighlightPossibleMoves(index);
                }
                else
                {
                    RedPlayerTurn = !RedPlayerTurn;
                    CurrentPlayerCanTakePiece = CanCurrentPlayerTakePiece();
                }
                break;
            default:
                RemoveHighlightedTiles();
                break;
        }
    }

    private bool PlayerCanMovePiece()
    {
        return ConnectedPlayers < 2 || ID == 0 && RedPlayerTurn || ID == 1 && !RedPlayerTurn;
    }

    public int GetTileIndex(int row, int column)
    {
        return row * 8 + column;
    }
    public (int, int) GetTileCoordinates(int index)
    {
        return (index / 8, index % 8);
    }
    public (bool, bool) GetDirectionsCheckerCanMove(char checkerType)
    {
        bool canMoveUp = checkerType == 'r' || char.IsUpper(checkerType);
        bool canMoveDown = checkerType == 'b' || char.IsUpper(checkerType);
        return (canMoveUp, canMoveDown);
    }
    private List<int> GetMoves(int index, char checkerType, bool calculateAttackingMoves)
    {
        List<int> moves = new();

        (bool canMoveUp, bool canMoveDown) = GetDirectionsCheckerCanMove(checkerType);
        if (canMoveUp)
            AttemptToAddMoveRows(index, calculateAttackingMoves, moves, -1);
        if (canMoveDown)
            AttemptToAddMoveRows(index, calculateAttackingMoves, moves, 1);

        return moves;
    }

    private void AttemptToAddMoveRows(int index, bool calculateAttackingMoves, List<int> moves, int rowShift)
    {
        AttemptToAddMove(moves, index, rowShift, -1, calculateAttackingMoves);
        AttemptToAddMove(moves, index, rowShift, 1, calculateAttackingMoves);
    }

    private void AttemptToAddMove(List<int> moves, int index, int rowShift, int columnShift, bool calculateAttackingMoves)
    {
        if (TryShiftCoordinates(index, rowShift, columnShift, out index))
        {
            if (CurrentPlayerCanTakePiece && calculateAttackingMoves)
            {
                if (char.ToLower(BoardTiles[index]) == OpposingCheckerType)
                {
                    if (TryShiftCoordinates(index, rowShift, columnShift, out index))
                    {
                        moves.Add(index);
                    }
                }
                return;
            }
            moves.Add(index);
        }
    }
    private bool TryShiftCoordinates(int index, int rowShift, int columnShift, out int newIndex)
    {
        (int row, int column) = GetTileCoordinates(index);
        if (AreRowAndColumnInBounds(row + rowShift, column + columnShift))
        {
            newIndex = GetTileIndex(row + rowShift, column + columnShift);
            return true;
        }
        newIndex = -1;
        return false;
    }
    private bool AreRowAndColumnInBounds(int row, int column)
    {
        return row >= 0 && row < 8 && column >= 0 && column < 8;
    }
    private bool CanCurrentPlayerTakePiece()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (char.ToLower(BoardTiles[i]) == IntendedCheckerType && CanCheckerTakeAnyPiece(i))
            {
                return true;
            }
        }
        return false;
    }
    private bool CanCheckerTakeAnyPiece(int index)
    {
        List<int> moves = GetMoves(index, BoardTiles[index], false);
        foreach (int move in moves)
        {
            if (char.ToLower(BoardTiles[move]) == OpposingCheckerType)
            {
                (int row, int column) = GetTileCoordinates(index);
                (int moveRow, int moveColumn) = GetTileCoordinates(move);
                int columnShift = moveColumn - column;
                int rowShift = moveRow - row;

                bool inBounds = TryShiftCoordinates(move, rowShift, columnShift, out int newIndex);
                if (inBounds && BoardTiles[newIndex] == ' ')
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void RemoveHighlightedTiles()
    {
        for (int i = 0; i < BoardTiles.Length; i++)
        {
            if (BoardTiles[i] == 'G')
            {
                UpdateBoardTile(i, ' ');
            }
        }
    }
    private void HighlightPossibleMoves(int index)
    {
        char checkerType = BoardTiles[index];
        if (char.ToLower(checkerType) == IntendedCheckerType)
        {
            SelectedTileIndex = index;
            List<int> moves = GetMoves(index, checkerType, true);
            moves.ForEach(move =>
            {
                if (BoardTiles[move] is ' ' or 'G')
                {
                    UpdateBoardTile(move, 'G');
                }
            });
        }
    }
    private void MovePiece(int finalTileIndex)
    {
        (int row, int column) = GetTileCoordinates(SelectedTileIndex);
        (int newRow, int newColumn) = GetTileCoordinates(finalTileIndex);
        int horizontalIncrement = row < newRow
                                    ? 1
                                    : -1;
        int verticalIncrement = column < newColumn
                                    ? 1
                                    : -1;

        while (SelectedTileIndex != finalTileIndex)
        {
            TryShiftCoordinates(SelectedTileIndex, horizontalIncrement, verticalIncrement, out int tileToMoveToIndex);

            UpdateBoardTile(tileToMoveToIndex, BoardTiles[SelectedTileIndex]);
            UpdateBoardTile(SelectedTileIndex, ' ');

            SelectedTileIndex = tileToMoveToIndex;
        }

        AttemptToCrownChecker(newRow, horizontalIncrement);
    }

    private void AttemptToCrownChecker(int row, int horizontalIncrement)
    {
        bool isMovingUp = horizontalIncrement < 0;
        bool hasReachedEnd = row == (isMovingUp ? 0 : 7);
        if (hasReachedEnd)
        {
            UpdateBoardTile(SelectedTileIndex, char.ToUpper(BoardTiles[SelectedTileIndex]));
        }
    }

    internal void UpdateBoardTile(int index, char value)
    {
        BoardTiles[index] = value;
        ColorChanged?.Invoke(this, new UpdateCheckerColorEventArgs(value, index));
    }
}
