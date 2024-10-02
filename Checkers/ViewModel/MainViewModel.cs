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
                if (canCurrentPlayerTakePiece && CanCheckerTakePiece(index))
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
    public int GetBoardIndex(int row, int column)
    {
        return row * 8 + column;
    }
    public (int, int) GetRowCoordinates(int index)
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
        (bool canMoveUp, bool canMoveDown) = GetDirectionsCheckerCanMove(checkerType);
        List<int> moves = new();

        if (canMoveUp)
        {
            AttemptToAddMove(moves, index, -1, -1, calculateAttackingMoves);
            AttemptToAddMove(moves, index, -1, 1, calculateAttackingMoves);
        }
        if (canMoveDown)
        {
            AttemptToAddMove(moves, index, 1, -1, calculateAttackingMoves);
            AttemptToAddMove(moves, index, 1, 1, calculateAttackingMoves);
        }
        return moves;
    }
    private void AttemptToAddMove(List<int> moves, int index, int rowShift, int columnShift, bool calculateAttackingMoves)
    {
        (int row, int column) = GetRowCoordinates(index);
        if (AreRowAndColumnValid(row + rowShift, column + columnShift))
        {
            index = GetBoardIndex(row + rowShift, column + columnShift);
            if (canCurrentPlayerTakePiece && calculateAttackingMoves)
            {
                if (char.ToLower(Board[index]) == opposingCheckerType)
                {
                    (row, column) = GetRowCoordinates(index);
                    if (AreRowAndColumnValid(row + rowShift, column + columnShift))
                    {
                        index = GetBoardIndex(row + rowShift, column + columnShift);
                        moves.Add(index);
                    }
                }
                return;
            }
            moves.Add(index);
        }
    }
    private bool AreRowAndColumnValid(int row, int column)
    {
        return row >= 0 && row < 8 && column >= 0 && column < 8;
    }
    private bool CanCurrentPlayerTakePiece()
    {
        for (int i = 0; i < Board.Length; i++)
        {
            if (char.ToLower(Board[i]) == intendedCheckerType)
            {
                if (CanCheckerTakePiece(i))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanCheckerTakePiece(int index)
    {
        List<int> moves = GetMoves(index, Board[index], false);
        foreach (int move in moves)
        {
            if (char.ToLower(Board[move]) == opposingCheckerType)
            {
                (int row, int column) = GetRowCoordinates(index);
                (int moveRow, int moveColumn) = GetRowCoordinates(move);
                int columnShift = moveColumn - column;
                int rowShift = moveRow - row;
                if (AreRowAndColumnValid(moveRow + rowShift, moveColumn + columnShift))
                {
                    if (Board[GetBoardIndex(moveRow + rowShift, moveColumn + columnShift)] == ' ')
                    {
                        return true;
                    }
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
            (int row, int column) = GetRowCoordinates(selectedTileIndex);

            List<int> moves = GetMoves(index, checkerType, true);
            foreach (int move in moves)
            {
                AttemptToHighlightTile(move);
            }
        }
    }
    private void MovePiece(int finalTileIndex)
    {
        (int row, int column) = GetRowCoordinates(selectedTileIndex);
        (int newRow, int newColumn) = GetRowCoordinates(finalTileIndex);
        int horizontalIncrement = row < newRow
                                    ? 1
                                    : -1;
        int verticalIncrement = column < newColumn
                                    ? 1
                                    : -1;

        while (selectedTileIndex != finalTileIndex)
        {
            int tileToMoveToIndex = GetBoardIndex(row + horizontalIncrement, column + verticalIncrement);

            UpdateBoardTile(tileToMoveToIndex, Board[selectedTileIndex]);
            UpdateBoardTile(selectedTileIndex, ' ');

            selectedTileIndex = tileToMoveToIndex;
            (row, column) = GetRowCoordinates(selectedTileIndex);
        }

        bool isMovingUp = horizontalIncrement < 0;
        bool hasReachedEnd = row == (isMovingUp ? 0 : 7);
        if (hasReachedEnd)
        {
            UpdateBoardTile(selectedTileIndex, char.ToUpper(Board[selectedTileIndex]));
        }
    }
    internal bool AttemptToHighlightTile(int row, int column)
    {
        return AttemptToHighlightTile(GetBoardIndex(row, column));
    }
    internal bool AttemptToHighlightTile(int index)
    {
        if (Board[index] is ' ' or 'G')
        {
            UpdateBoardTile(index, 'G');
            return true;
        }
        return intendedCheckerType == Board[index];
    }
    internal void UpdateBoardTile(int index, char value)
    {
        Board[index] = value;
        ColorChanged?.Invoke(this, new UpdateCheckerColorEventArgs(value, index));
    }
}