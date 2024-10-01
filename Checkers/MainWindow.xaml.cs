using Checkers.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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
        Grid grid = (Grid)sender;
        DependencyObject dataTemplate = VisualTreeHelper.GetParent(grid);
        DependencyObject uniformGrid = VisualTreeHelper.GetParent(dataTemplate);

        if (uniformGrid is UniformGrid parentGrid)
        {
            int index = parentGrid.Children.IndexOf((UIElement)dataTemplate);
        }
    }
}