using Checkers.ViewModel;
using System.Windows.Controls;

namespace Checkers.View;
/// <summary>
/// Interaction logic for Board.xaml
/// </summary>
public partial class CheckersBoard : UserControl
{
    private readonly CheckersBoardViewModel _viewModel = new();
    public CheckersBoard()
    {
        InitializeComponent();
    }
    private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _viewModel.Grid_MouseLeftButtonDown(sender, e);
    }
}
