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

  private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    FrameworkElement pnlClient = (this.Content as FrameworkElement)!;
    _viewModel.Window_SizeChanged(pnlClient.ActualWidth, pnlClient.ActualHeight);
  }
}
