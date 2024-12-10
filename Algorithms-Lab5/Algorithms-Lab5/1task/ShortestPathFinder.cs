using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algorithms_Lab5._1task;

public class ShortestPathFinder
{
    private Canvas _canvas;
    private TextBox _outputTextBox;

    public ShortestPathFinder(Canvas canvas, TextBox outputTextBox)
    {
        _canvas = canvas;
        _outputTextBox = outputTextBox;
    }

    public async Task FindShortestPath(Dictionary<int, List<(int, double)>> graph, int startNode, int endNode, Dictionary<int, Grid> nodes, List<Line> edges)
    {
        // Инициализация
        var distances = new Dictionary<int, double>();
        var previous = new Dictionary<int, int?>();
        var visited = new HashSet<int>();
        var priorityQueue = new SortedSet<(double distance, int node)>();

        foreach (var node in graph.Keys)
        {
            distances[node] = double.PositiveInfinity;
            previous[node] = null;
        }

        distances[startNode] = 0;
        priorityQueue.Add((0, startNode));

        _outputTextBox.Clear();
        _outputTextBox.AppendText($"Поиск кратчайшего пути от узла {startNode} к узлу {endNode} начат.\n");

        // Алгоритм Дейкстры
        while (priorityQueue.Count > 0)
        {
            var (currentDistance, currentNode) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            if (currentNode == endNode)
            {
                _outputTextBox.AppendText("Кратчайший путь найден.\n");
                await HighlightPath(previous, endNode, nodes, edges);
                return;
            }

            if (visited.Contains(currentNode))
                continue;

            visited.Add(currentNode);
            HighlightNode(nodes[currentNode]);
            _outputTextBox.AppendText($"Посещаем узел {currentNode} с текущей длиной пути {currentDistance}.\n");

            foreach (var (neighbor, weight) in graph[currentNode])
            {
                if (visited.Contains(neighbor)) continue;

                double newDistance = distances[currentNode] + weight;

                if (newDistance < distances[neighbor])
                {
                    priorityQueue.Remove((distances[neighbor], neighbor));
                    distances[neighbor] = newDistance;
                    previous[neighbor] = currentNode;
                    priorityQueue.Add((newDistance, neighbor));
                    _outputTextBox.AppendText($"Обновляем длину пути до узла {neighbor}: {newDistance} через узел {currentNode}.\n");
                }
            }

            await Task.Delay(1000); // Для визуализации
        }

        _outputTextBox.AppendText("Кратчайший путь не найден (граф несвязный).\n");
    }

    private async Task HighlightPath(Dictionary<int, int?> previous, int endNode, Dictionary<int, Grid> nodes, List<Line> edges)
    {
        var path = new List<int>();
        int? currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Value);
            currentNode = previous[currentNode.Value];
        }

        path.Reverse();

        _outputTextBox.AppendText("Кратчайший путь: " + string.Join(" -> ", path) + "\n");

        for (int i = 0; i < path.Count - 1; i++)
        {
            int start = path[i];
            int end = path[i + 1];

            HighlightNode(nodes[start]);
            HighlightNode(nodes[end]);

            var edge = edges.FirstOrDefault(e => IsEdgeBetweenNodes(e, nodes[start], nodes[end]));
            if (edge != null)
            {
                HighlightEdge(edge);
            }

            await Task.Delay(500); // Для визуализации
        }
    }

    private void HighlightNode(Grid node)
    {
        var ellipse = node.Children[0] as Ellipse;
        if (ellipse != null)
        {
            ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
        }
    }

    private void HighlightEdge(Line edge)
    {
        edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
    }

    private bool IsEdgeBetweenNodes(Line edge, Grid startNode, Grid endNode)
    {
        Point startCenter = GetNodeCenter(startNode);
        Point endCenter = GetNodeCenter(endNode);

        return (Math.Abs(edge.X1 - startCenter.X) < 1e-5 && Math.Abs(edge.Y1 - startCenter.Y) < 1e-5 &&
                Math.Abs(edge.X2 - endCenter.X) < 1e-5 && Math.Abs(edge.Y2 - endCenter.Y) < 1e-5) ||
               (Math.Abs(edge.X1 - endCenter.X) < 1e-5 && Math.Abs(edge.Y1 - endCenter.Y) < 1e-5 &&
                Math.Abs(edge.X2 - startCenter.X) < 1e-5 && Math.Abs(edge.Y2 - startCenter.Y) < 1e-5);
    }

    private Point GetNodeCenter(Grid node)
    {
        double left = Canvas.GetLeft(node);
        double top = Canvas.GetTop(node);
        double centerX = left + 35;
        double centerY = top + 35;
        return new Point(centerX, centerY);
    }
}