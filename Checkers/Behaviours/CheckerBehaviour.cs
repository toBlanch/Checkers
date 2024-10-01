using Checkers.ViewModel;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Checkers.Behaviours;
public class CheckerPieceEventArgs
{
    public char item;
    public int index;

    public CheckerPieceEventArgs(char item, int index)
    {
        this.item = item;
        this.index = index;
    }
}

public delegate void ColorChangedEventhandler(object self, CheckerPieceEventArgs e);
internal class CheckerBehaviour : Behavior<Ellipse>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        MainViewModel.ColorChanged += UpdateColor;
    }
    protected override void OnDetaching()
    {
        base.OnDetaching();
    }
    public void UpdateColor(object sender, CheckerPieceEventArgs e)
    {

        DependencyObject grid = VisualTreeHelper.GetParent(AssociatedObject);
        DependencyObject dataTemplate = VisualTreeHelper.GetParent(grid);
        DependencyObject uniformGrid = VisualTreeHelper.GetParent(dataTemplate);

        if (uniformGrid is UniformGrid parentGrid)
        {
            int index = parentGrid.Children.IndexOf((UIElement)dataTemplate);
            if (index == e.index)
            {
                AssociatedObject.Fill = e.item switch
                {
                    'b' => Brushes.Black,
                    'r' => Brushes.Red,
                    'w' => Brushes.White,
                    _ => Brushes.Transparent,
                };
            }
        }
    }
}
