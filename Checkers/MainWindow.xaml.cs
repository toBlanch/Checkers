using Checkers.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Checkers;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();
    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Grid grid = (Grid)sender;
        int index = Grid.GetRow(grid) * 8 + Grid.GetColumn(grid);
    }
}