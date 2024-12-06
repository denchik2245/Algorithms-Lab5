using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algorithms_Lab5.Tools
{
    public class Move
    {
        private bool _isDragging = false;
        private Point _startPoint;
        private Grid _selectedNode;
        private Canvas _canvas;

        public bool IsActive { get; set; }

        public void Initialize(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void StartDrag(MouseButtonEventArgs e)
        {
            if (!IsActive || _canvas == null) return;
            
            HitTestResult hitResult = VisualTreeHelper.HitTest(_canvas, e.GetPosition(_canvas));
            if (hitResult?.VisualHit is Ellipse && VisualTreeHelper.GetParent(hitResult.VisualHit) is Grid grid)
            {
                _isDragging = true;
                _selectedNode = grid;
                _startPoint = e.GetPosition(_canvas);
            }
        }

        public void Drag(MouseEventArgs e)
        {
            if (_isDragging && _selectedNode != null)
            {
                Point currentPoint = e.GetPosition(_canvas);
                
                double offsetX = currentPoint.X - _startPoint.X;
                double offsetY = currentPoint.Y - _startPoint.Y;
                
                double newLeft = Canvas.GetLeft(_selectedNode) + offsetX;
                double newTop = Canvas.GetTop(_selectedNode) + offsetY;
                Canvas.SetLeft(_selectedNode, newLeft);
                Canvas.SetTop(_selectedNode, newTop);
                
                UpdateEdges(_selectedNode, newLeft, newTop);
                
                _startPoint = currentPoint;
            }
        }

        public void EndDrag()
        {
            if (_isDragging)
            {
                _isDragging = false;
                _selectedNode = null;
            }
        }

        private void UpdateEdges(Grid node, double newLeft, double newTop)
        {
            double centerX = newLeft + (node.ActualWidth / 2);
            double centerY = newTop + (node.ActualHeight / 2);
            
            foreach (UIElement element in _canvas.Children)
            {
                if (element is Line line)
                {
                    if (line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                    {
                        Grid startNode = connectedNodes.Item1;
                        Grid endNode = connectedNodes.Item2;
                        TextBlock weightTextBlock = connectedNodes.Item3;
                        
                        if (startNode == node)
                        {
                            line.X1 = centerX;
                            line.Y1 = centerY;
                        }
                        
                        if (endNode == node)
                        {
                            line.X2 = centerX;
                            line.Y2 = centerY;
                        }
                        
                        Canvas.SetLeft(weightTextBlock, (line.X1 + line.X2) / 2 - weightTextBlock.ActualWidth / 2);
                        Canvas.SetTop(weightTextBlock, (line.Y1 + line.Y2) / 2 - weightTextBlock.ActualHeight / 2);
                    }
                }
            }
        }
    }
}
