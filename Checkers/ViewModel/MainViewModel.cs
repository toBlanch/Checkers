using CanvasRect;
using Checkers.Behaviours;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Checkers.ViewModel;


class MainViewModel : CanvasRectBase
{
    public static event ColorChangedEventhandler? ColorChanged;
    public char[] Board { get; } = new char[64];
    int selectedTileIndex = -1;
    bool redPlayerTurn = true;
    bool canCurrentPlayerTakePiece = false;
    char intendedCheckerType => redPlayerTurn
                                ? 'r'
                                : 'b';
    char opposingCheckerType => redPlayerTurn
                                ? 'b'
                                : 'r';


    public MainViewModel()
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
    public void Window_SizeChanged(double actualWidth, double actualHeight)
    {
        Graphics.window = new Vector(actualWidth, actualHeight); // Updates the window size
        RenderCanvasRect();
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
        switch (char.ToLower(Board[index]))
        {
            case 'b':
            case 'r':
                RemoveHighlightedTiles();
                HighlightPossibleMoves(index);
                break;
            case 'g':
                MovePiece(index);
                RemoveHighlightedTiles();
                if (canCurrentPlayerTakePiece && CanCheckerTakeAnyPiece(index))
                {
                    selectedTileIndex = index;
                    HighlightPossibleMoves(index);
                }
                else
                {
                    redPlayerTurn = !redPlayerTurn;
                    canCurrentPlayerTakePiece = CanCurrentPlayerTakePiece();
                }
                break;
            default:
                RemoveHighlightedTiles();
                break;
        }
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
            if (canCurrentPlayerTakePiece && calculateAttackingMoves)
            {
                if (char.ToLower(Board[index]) == opposingCheckerType)
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
        for (int i = 0; i < Board.Length; i++)
        {
            if (char.ToLower(Board[i]) == intendedCheckerType && CanCheckerTakeAnyPiece(i))
            {
                return true;
            }
        }
        return false;
    }
    private bool CanCheckerTakeAnyPiece(int index)
    {
        List<int> moves = GetMoves(index, Board[index], false);
        foreach (int move in moves)
        {
            if (char.ToLower(Board[move]) == opposingCheckerType)
            {
                (int row, int column) = GetTileCoordinates(index);
                (int moveRow, int moveColumn) = GetTileCoordinates(move);
                int columnShift = moveColumn - column;
                int rowShift = moveRow - row;

                bool inBounds = TryShiftCoordinates(move, rowShift, columnShift, out int newIndex);
                if (inBounds && Board[newIndex] == ' ')
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void RemoveHighlightedTiles()
    {
        for (int i = 0; i < Board.Length; i++)
        {
            if (Board[i] == 'G')
            {
                UpdateBoardTile(i, ' ');
            }
        }
    }
    private void HighlightPossibleMoves(int index)
    {
        char checkerType = Board[index];
        if (char.ToLower(checkerType) == intendedCheckerType)
        {
            selectedTileIndex = index;
            List<int> moves = GetMoves(index, checkerType, true);
            moves.ForEach(move =>
            {
                if (Board[move] is ' ' or 'G')
                {
                    UpdateBoardTile(move, 'G');
                }
            });
        }
    }
    private void MovePiece(int finalTileIndex)
    {
        (int row, int column) = GetTileCoordinates(selectedTileIndex);
        (int newRow, int newColumn) = GetTileCoordinates(finalTileIndex);
        int horizontalIncrement = row < newRow
                                    ? 1
                                    : -1;
        int verticalIncrement = column < newColumn
                                    ? 1
                                    : -1;

        while (selectedTileIndex != finalTileIndex)
        {
            TryShiftCoordinates(selectedTileIndex, horizontalIncrement, verticalIncrement, out int tileToMoveToIndex);

            UpdateBoardTile(tileToMoveToIndex, Board[selectedTileIndex]);
            UpdateBoardTile(selectedTileIndex, ' ');

            selectedTileIndex = tileToMoveToIndex;
        }

        AttemptToCrownChecker(newRow, horizontalIncrement);
    }

    private void AttemptToCrownChecker(int row, int horizontalIncrement)
    {
        bool isMovingUp = horizontalIncrement < 0;
        bool hasReachedEnd = row == (isMovingUp ? 0 : 7);
        if (hasReachedEnd)
        {
            UpdateBoardTile(selectedTileIndex, char.ToUpper(Board[selectedTileIndex]));
        }
    }

    internal void UpdateBoardTile(int index, char value)
    {
        Board[index] = value;
        ColorChanged?.Invoke(this, new UpdateCheckerColorEventArgs(value, index));
    }
}