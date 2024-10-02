using CanvasRect;
using Checkers.Behaviours;
using Checkers.Model;
using Checkers.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Checkers.ViewModel;


class MainViewModel : CanvasRectBase
{
    public static event ColorChangedEventhandler? ColorChanged;
    private readonly Main _main = new();
    public char[] Board
    {
        get => _main.Board;
        set => _main.Board = value;
    }
    public int SelectedTileIndex
    {
        get => _main.SelectedTileIndex;
        set => _main.SelectedTileIndex = value;
    }
    public bool RedPlayerTurn
    {
        get => _main.RedPlayerTurn;
        set => _main.RedPlayerTurn = value;
    }
    public bool CurrentPlayerCanTakePiece
    {
        get => _main.CurrentPlayerCanTakePiece;
        set => _main.CurrentPlayerCanTakePiece = value;
    }
    char IntendedCheckerType => RedPlayerTurn
                                ? 'r'
                                : 'b';
    char OpposingCheckerType => RedPlayerTurn
                                ? 'b'
                                : 'r';

    private readonly Server _server;
    public MainViewModel()
    {
        _server = new();
        _server.ConnectToServer();
        //using TcpClient client = new();
        //string hostname = "127.0.0.1";
        //client.Connect(hostname, 13000);
        //NetworkStream stream = client.GetStream();
        //string message = "Hello World\r\n";
        //Console.WriteLine(message);
        //using StreamReader reader = new(stream, Encoding.UTF8);
        //byte[] bytes = Encoding.UTF8.GetBytes(message);
        //stream.Write(bytes, 0, bytes.Length);
        //Console.WriteLine(reader.ReadToEnd());

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
                if (char.ToLower(Board[index]) == OpposingCheckerType)
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
            if (char.ToLower(Board[i]) == IntendedCheckerType && CanCheckerTakeAnyPiece(i))
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
            if (char.ToLower(Board[move]) == OpposingCheckerType)
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
        if (char.ToLower(checkerType) == IntendedCheckerType)
        {
            SelectedTileIndex = index;
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

            UpdateBoardTile(tileToMoveToIndex, Board[SelectedTileIndex]);
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
            UpdateBoardTile(SelectedTileIndex, char.ToUpper(Board[SelectedTileIndex]));
        }
    }

    internal void UpdateBoardTile(int index, char value)
    {
        Board[index] = value;
        ColorChanged?.Invoke(this, new UpdateCheckerColorEventArgs(value, index));
    }
}