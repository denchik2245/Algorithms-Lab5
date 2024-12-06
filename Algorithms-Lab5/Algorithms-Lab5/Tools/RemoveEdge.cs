using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algorithms_Lab5.Tools
{
    public class RemoveEdge
    {
        public bool IsActive { get; set; }

        public void Remove(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!IsActive) return;
            
            HitTestResult hitResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            if (hitResult?.VisualHit is Line edge)
            {
                if (edge.Tag is Tuple<Grid, Grid, TextBlock> taggedNodesWithWeight)
                {
                    TextBlock weightTextBlock = taggedNodesWithWeight.Item3;
                    canvas.Children.Remove(weightTextBlock);
                }
                
                canvas.Children.Remove(edge);
            }
        }
    }
}