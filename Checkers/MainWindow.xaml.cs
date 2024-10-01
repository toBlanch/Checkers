using Checkers.ViewModel;
using System.Windows;

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

    private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _viewModel.Grid_MouseLeftButtonDown(sender, e);
    }
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _viewModel.Window_SizeChanged(ActualWidth, ActualHeight - SystemParameters.CaptionHeight);
    }
}