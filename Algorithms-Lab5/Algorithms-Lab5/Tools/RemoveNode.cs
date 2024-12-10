using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
   public class RemoveNode
    {
        public bool IsActive { get; set; }
        private readonly GraphData _graphData;

        public RemoveNode(GraphData graphData)
        {
            _graphData = graphData;
        }

        public void Remove(Canvas canvas, MouseButtonEventArgs e)
        {
            if (!IsActive) return;
            
            Point clickPosition = e.GetPosition(canvas);
            Grid nodeToRemove = null;

            foreach (UIElement element in canvas.Children)
            {
                if (element is Grid grid)
                {
                    double left = Canvas.GetLeft(grid);
                    double top = Canvas.GetTop(grid);
                    double right = left + grid.ActualWidth;
                    double bottom = top + grid.ActualHeight;

                    if (clickPosition.X >= left && clickPosition.X <= right &&
                        clickPosition.Y >= top && clickPosition.Y <= bottom)
                    {
                        nodeToRemove = grid;
                        break;
                    }
                }
            }

            if (nodeToRemove == null)
            {
                MessageBox.Show("Узел для удаления не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            RemoveConnectedEdges(canvas, nodeToRemove);
            
            if (nodeToRemove.Tag is string nodeLabel)
            {
                _graphData.RemoveNode(nodeLabel);
            }
            
            canvas.Children.Remove(nodeToRemove);
        }

        private void RemoveConnectedEdges(Canvas canvas, Grid node)
        {
            var elementsToRemove = new List<UIElement>();
            foreach (UIElement element in canvas.Children.OfType<UIElement>().ToList())
            {
                if (element is Line line && line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                {
                    if (connectedNodes.Item1 == node || connectedNodes.Item2 == node)
                    {
                        elementsToRemove.Add(line);
                        
                        if (connectedNodes.Item3 is TextBlock weightTextBlock)
                        {
                            elementsToRemove.Add(weightTextBlock);
                        }
                        
                        string firstLabel = connectedNodes.Item1.Tag as string;
                        string secondLabel = connectedNodes.Item2.Tag as string;

                        if (!string.IsNullOrEmpty(firstLabel) && !string.IsNullOrEmpty(secondLabel))
                        {
                            _graphData.RemoveEdge(firstLabel, secondLabel);
                        }
                    }
                }
            }
            
            foreach (var element in elementsToRemove)
            {
                canvas.Children.Remove(element);
            }
        }
    }

}
