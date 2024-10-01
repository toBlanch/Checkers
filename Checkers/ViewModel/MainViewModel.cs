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
    private char[] board = new char[64];
    public char[] Board
    {
        get
        {
            return board;
        }
        set
        {
            board = value;
            RaisePropertyChanged();
        }
    }
    int selectedTileIndex = -1;


    public MainViewModel()
    {
        Width = 800;
        Height = 600;
        for (int i = 0; i < 64; i++)
        {
            bool isOnBlackTile = i % 2 == i / 8 % 2;
            bool isOnTopThreeRows = i < 24;
            Board[i] = isOnBlackTile
                ? ' '
                : isOnTopThreeRows
                    ? 'b'
                    : i > 39
                        ? 'r'
                        : ' ';
        }
    }
    public void Window_SizeChanged(double actualWidth, double actualHeight)
    {
        Graphics.window = new Vector(actualWidth, actualHeight); // Updates the window size
        RenderCanvasRect();
    }
    public int GetBoardIndex(int row, int column)
    {
        return row * 8 + column;
    }
    public (int, int) GetRowCoordinates(int index)
    {
        return (index / 8, index % 8);
    }

    internal void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Grid grid = (Grid)sender;
        DependencyObject dataTemplate = VisualTreeHelper.GetParent(grid);
        DependencyObject uniformGrid = VisualTreeHelper.GetParent(dataTemplate);

        if (uniformGrid is UniformGrid parentGrid)
        {
            int index = parentGrid.Children.IndexOf((UIElement)dataTemplate);

            switch (Board[index])
            {
                case 'b':
                case 'r':
                    selectedTileIndex = index;
                    Board.Select(b => b == 'G' ? ' ' : b).ToArray();
                    (int row, int column) = GetRowCoordinates(index);
                    if (!AttemptToHighlightTile(row - 1, column - 1))
                    {
                        AttemptToHighlightTile(row - 2, column - 2);
                    }
                    if (!AttemptToHighlightTile(row + 1, column - 1))
                    {
                        AttemptToHighlightTile(row + 2, column - 2);
                    }
                    break;
                case 'G':
                    MovePiece(index);
                    break;
            }

            //Temporary board changing condition
            ColorChanged?.Invoke(this, new CheckerPieceEventArgs(' ', index));
        }
    }

    private void MovePiece(int index)
    {
        (int row, int column) = GetRowCoordinates(selectedTileIndex);
        (int newRow, int newColumn) = GetRowCoordinates(index);
        int horizontalIncrement = row < newRow
            ? 1
            : -1;
        int verticalIncrement = column < newColumn
            ? 1
            : -1;
        while (selectedTileIndex != index)
        {
            int tileToMoveToIndex = GetBoardIndex(row + horizontalIncrement, column + verticalIncrement);
            board[tileToMoveToIndex] = board[selectedTileIndex];
            board[selectedTileIndex] = ' ';
            selectedTileIndex = tileToMoveToIndex;
        }
    }

    internal bool IfTileIsFree(int row, int column)
    {
        if (row < 0 || row > 7 || column < 0 || column > 7)
        {
            return false;
        }
        return Board[GetBoardIndex(row, column)] is ' ' or 'G';
    }
    internal bool AttemptToHighlightTile(int row, int column)
    {
        if (IfTileIsFree(row, column))
        {
            Board[GetBoardIndex(row, column)] = 'G';
            return true;
        }
        return false;
    }
}

