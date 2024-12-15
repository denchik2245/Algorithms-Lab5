using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Tools;

public class AddDirectedEdge
{
    private Grid _firstNode;
    private bool _isSelectingFirstNode = true;
    public bool IsActive { get; set; }
    private readonly GraphData _graphData;
    private bool _isTransportNetworkMode;

    public AddDirectedEdge(GraphData graphData)
    {
        _graphData = graphData;
    }

    public void SetTransportNetworkMode(bool isTransportNetworkMode)
    {
        _isTransportNetworkMode = isTransportNetworkMode;
    }
    
    public void SelectNode(Canvas canvas, UIElement clickedElement)
    {
        if (!IsActive || clickedElement is not Grid node) return;

        if (_isSelectingFirstNode)
        {
            _firstNode = node;
            _isSelectingFirstNode = false;
        }
        else
        {
            Point firstNodeCenter = GetNodeCenter(_firstNode);
            Point secondNodeCenter = GetNodeCenter(node);

            const double nodeRadius = 35;

            Point startPoint = GetPointOnCircleBorder(firstNodeCenter, secondNodeCenter, nodeRadius);
            Point endPoint = GetPointOnCircleBorder(secondNodeCenter, firstNodeCenter, nodeRadius);

            string firstLabel = (string)_firstNode.Tag;
            string secondLabel = (string)node.Tag;

            TextBlock weightText = CreateWeightTextBlock("0", canvas);
            
            Line edge = DrawEdgeWithWeight(canvas, _firstNode, node, weightText, isDirected: _isTransportNetworkMode);

            _graphData.AddEdge(firstLabel, secondLabel, 0, edge, weightText);

            _firstNode = null;
            _isSelectingFirstNode = true;
        }
    }

    private Point GetNodeCenter(Grid node)
    {
        double left = Canvas.GetLeft(node);
        double top = Canvas.GetTop(node);
        double centerX = left + node.ActualWidth / 2;
        double centerY = top + node.ActualHeight / 2;
        return new Point(centerX, centerY);
    }

    private Point GetPointOnCircleBorder(Point circleCenter, Point targetPoint, double radius)
    {
        double dx = targetPoint.X - circleCenter.X;
        double dy = targetPoint.Y - circleCenter.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance == 0) return circleCenter;

        double normalizedX = dx / distance;
        double normalizedY = dy / distance;

        double borderX = circleCenter.X + normalizedX * radius;
        double borderY = circleCenter.Y + normalizedY * radius;

        return new Point(borderX, borderY);
    }

    public Line DrawEdgeWithWeight(Canvas canvas, Grid firstNode, Grid secondNode, TextBlock weightText, bool isDirected = false)
    {
        Point firstNodeCenter = GetNodeCenter(firstNode);
        Point secondNodeCenter = GetNodeCenter(secondNode);

        const double nodeRadius = 35;
        
        Point startPoint = GetPointOnCircleBorder(firstNodeCenter, secondNodeCenter, nodeRadius);
        Point endPoint = GetPointOnCircleBorder(secondNodeCenter, firstNodeCenter, nodeRadius);
        
        Line edge = new Line
        {
            X1 = startPoint.X,
            Y1 = startPoint.Y,
            X2 = endPoint.X,
            Y2 = endPoint.Y,
            Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2D32")),
            StrokeThickness = 5
        };

        Polyline arrow = null;
        
        if (isDirected)
        {
            arrow = DrawArrow(canvas, edge, startPoint, endPoint);
            arrow.Tag = edge;
        }
        
        edge.Tag = Tuple.Create(firstNode, secondNode, weightText, arrow);
        
        double midX = (startPoint.X + endPoint.X) / 2;
        double midY = (startPoint.Y + endPoint.Y) / 2;
        Canvas.SetLeft(weightText, midX - weightText.ActualWidth / 2);
        Canvas.SetTop(weightText, midY - weightText.ActualHeight / 2);

        weightText.Tag = edge;
        
        canvas.Children.Add(edge);
        if (arrow != null)
        {
            canvas.Children.Add(arrow);
        }
        canvas.Children.Add(weightText);

        return edge;
    }

    private Polyline DrawArrow(Canvas canvas, Line edge, Point start, Point end)
    {
        const double arrowSize = 20;
        double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        
        Point arrowBase = new Point(
            end.X - arrowSize * Math.Cos(angle),
            end.Y - arrowSize * Math.Sin(angle)
        );
        
        Point arrowPoint1 = new Point(arrowBase.X - arrowSize * Math.Cos(angle - Math.PI / 6),
            arrowBase.Y - arrowSize * Math.Sin(angle - Math.PI / 6));
        Point arrowPoint2 = new Point(arrowBase.X - arrowSize * Math.Cos(angle + Math.PI / 6),
            arrowBase.Y - arrowSize * Math.Sin(angle + Math.PI / 6));
        
        Polyline arrow = new Polyline
        {
            Points = new PointCollection { end, arrowPoint1, arrowPoint2 },
            Stroke = edge.Stroke,
            StrokeThickness = edge.StrokeThickness,
            Fill = edge.Stroke
        };

        return arrow;
    }

    public TextBlock CreateWeightTextBlock(string weight, Canvas canvas)
    {
        TextBlock weightText = new TextBlock
        {
            Text = weight,
            Foreground = Brushes.Black,
            Background = Brushes.White,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Padding = new Thickness(2)
        };

        weightText.MouseLeftButtonDown += (sender, args) =>
        {
            if (sender is TextBlock clickedTextBlock && clickedTextBlock.Tag is Line line)
            {
                // Проверяем сначала направленные ребра (4 элемента в кортеже)
                if (line.Tag is Tuple<Grid, Grid, TextBlock, Polyline> directedEdgeData)
                {
                    var (firstNode, secondNode, textBlock, arrow) = directedEdgeData;
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Введите новый вес ребра:", "Изменить вес", clickedTextBlock.Text);
                    if (double.TryParse(input, out double newWeight) && newWeight >= 0)
                    {
                        clickedTextBlock.Text = newWeight.ToString(CultureInfo.InvariantCulture);

                        string firstLabel = (string)firstNode.Tag;
                        string secondLabel = (string)secondNode.Tag;
                        _graphData.AddEdge(firstLabel, secondLabel, newWeight, line, clickedTextBlock);
                    }
                    else
                    {
                        MessageBox.Show("Некорректное значение веса. Введите неотрицательное число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (line.Tag is Tuple<Grid, Grid, TextBlock> undirectedEdgeData)
                {
                    var (firstNode, secondNode, textBlock) = undirectedEdgeData;
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Введите новый вес ребра:", "Изменить вес", clickedTextBlock.Text);
                    if (double.TryParse(input, out double newWeight) && newWeight >= 0)
                    {
                        clickedTextBlock.Text = newWeight.ToString(CultureInfo.InvariantCulture);

                        string firstLabel = (string)firstNode.Tag;
                        string secondLabel = (string)secondNode.Tag;
                        _graphData.AddEdge(firstLabel, secondLabel, newWeight, line, clickedTextBlock);
                    }
                    else
                    {
                        MessageBox.Show("Некорректное значение веса. Введите неотрицательное число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось определить связанное ребро.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить связанное ребро.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        };

        return weightText;
    }
}