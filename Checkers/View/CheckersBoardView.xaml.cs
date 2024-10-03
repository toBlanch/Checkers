using Checkers.ViewModel;
using System.Windows.Controls;

namespace Checkers.View;
/// <summary>
/// Interaction logic for Board.xaml
/// </summary>
public partial class CheckersBoardView : UserControl
{
  public static event CheckerPieceClickedEventhandler? CheckerPieceClicked;
  public CheckersBoardView()
  {
    InitializeComponent();
  }
  private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
    CheckerPieceClicked?.Invoke(sender, e);
  }
}
