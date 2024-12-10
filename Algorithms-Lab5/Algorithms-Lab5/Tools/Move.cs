using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools
{
    public class Move
    {
        private bool _isDragging = false;
        private Point _startPoint;
        private Grid _selectedNode;
        private Canvas _canvas;
        private Point _virtualPosition;
        private readonly GraphData _graphData;

        public bool IsActive { get; set; }

        public Move(GraphData graphData)
        {
            _graphData = graphData;
        }

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
                
                Canvas.SetZIndex(_selectedNode, 10);
            }
        }

        public void Drag(MouseEventArgs e)
        {
            if (_isDragging && _selectedNode != null)
            {
                Point currentPoint = e.GetPosition(_canvas);

                double offsetX = currentPoint.X - _startPoint.X;
                double offsetY = currentPoint.Y - _startPoint.Y;
                
                _virtualPosition = new Point(Canvas.GetLeft(_selectedNode) + offsetX, Canvas.GetTop(_selectedNode) + offsetY);
                
                Canvas.SetLeft(_selectedNode, _virtualPosition.X);
                Canvas.SetTop(_selectedNode, _virtualPosition.Y);
                
                UpdateEdges(_selectedNode, _virtualPosition.X, _virtualPosition.Y);
                _startPoint = currentPoint;
            }
        }

        public void EndDrag()
        {
            if (_isDragging)
            {
                _isDragging = false;
                
                if (_selectedNode != null && _selectedNode.Tag is string nodeLabel)
                {
                    double newX = _virtualPosition.X + (_selectedNode.ActualWidth / 2);
                    double newY = _virtualPosition.Y + (_selectedNode.ActualHeight / 2);
                    _graphData.UpdateNodePosition(nodeLabel, newX, newY);
                }
                
                UpdateEdges(_selectedNode, _virtualPosition.X, _virtualPosition.Y);
                
                Canvas.SetZIndex(_selectedNode, 1);
                _selectedNode = null;
            }
        }

        public void UpdateEdges(Grid node, double newLeft, double newTop)
        {
            double newCenterX = newLeft + (node.ActualWidth / 2);
            double newCenterY = newTop + (node.ActualHeight / 2);
            
            foreach (UIElement element in _canvas.Children)
            {
                if (element is Line line)
                {
                    if (line.Tag is Tuple<Grid, Grid, TextBlock> connectedNodes)
                    {
                        Grid startNode = connectedNodes.Item1;
                        Grid endNode = connectedNodes.Item2;
                        
                        if (startNode == node || endNode == node)
                        {
                            double offset = 5;

                            if (startNode == node)
                            {
                                line.X1 = newCenterX + offset;
                                line.Y1 = newCenterY + offset;
                            }

                            if (endNode == node)
                            {
                                line.X2 = newCenterX + offset;
                                line.Y2 = newCenterY + offset;
                            }
                            
                            TextBlock weightTextBlock = connectedNodes.Item3;
                            Canvas.SetLeft(weightTextBlock, (line.X1 + line.X2) / 2 - weightTextBlock.ActualWidth / 2);
                            Canvas.SetTop(weightTextBlock, (line.Y1 + line.Y2) / 2 - weightTextBlock.ActualHeight / 2);
                        }
                    }
                }
            }
        }
    }

}
