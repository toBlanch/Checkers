using Checkers.ViewModel;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Checkers.Behaviours;
public class UpdateCheckerColorEventArgs
{
    public char checkerType;
    public int index;

    public UpdateCheckerColorEventArgs(char item, int index)
    {
        this.checkerType = item;
        this.index = index;
    }
}

public delegate void ColorChangedEventhandler(object self, UpdateCheckerColorEventArgs e);
internal class CheckerBehaviour : Behavior<Ellipse>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        MainViewModel.ColorChanged += UpdateCheckerColor;
    }
    protected override void OnDetaching()
    {
        base.OnDetaching();
    }
    public void UpdateCheckerColor(object sender, UpdateCheckerColorEventArgs e)
    {

        DependencyObject grid = VisualTreeHelper.GetParent(AssociatedObject);
        DependencyObject dataTemplate = VisualTreeHelper.GetParent(grid);
        DependencyObject uniformGrid = VisualTreeHelper.GetParent(dataTemplate);

        if (uniformGrid is UniformGrid parentGrid)
        {
            int index = parentGrid.Children.IndexOf((UIElement)dataTemplate);
            if (index == e.index)
            {
                AssociatedObject.Fill = e.checkerType switch
                {
                    'b' => Brushes.Black,
                    'B' => Brushes.Bisque,
                    'r' => Brushes.Red,
                    'R' => Brushes.DarkRed,
                    'G' => Brushes.Green,
                    _ => Brushes.Transparent,
                };
            }
        }
    }
}
