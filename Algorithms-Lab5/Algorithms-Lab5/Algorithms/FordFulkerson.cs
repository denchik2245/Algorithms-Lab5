using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

public class FordFulkerson
{
    private TextBox OutputTextBox;
    private Dictionary<string, Dictionary<string, double>> residualCapacity;
    private Dictionary<string, Brush> originalNodeColors = new Dictionary<string, Brush>();

    public FordFulkerson(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task<double> Execute(GraphData graphData, string source, string sink)
    {
        InitializeResidualNetwork(graphData);

        double maxFlow = 0;

        OutputTextBox.AppendText($"\nНачинаем поиск максимального потока из {source} в {sink}.\n");

        while (true)
        {
            var parent = await FindAugmentingPath(graphData, source, sink);
            if (parent == null)
            {
                OutputTextBox.AppendText("Дополняющий путь не найден. Завершаем алгоритм.\n\n");
                break;
            }

            double pathFlow = double.PositiveInfinity;
            string v = sink;
            while (v != source)
            {
                string u = parent[v];
                pathFlow = Math.Min(pathFlow, residualCapacity[u][v]);
                v = u;
            }

            v = sink;
            var augmentingPath = new List<string>();
            while (v != source)
            {
                augmentingPath.Add(v);
                string u = parent[v];
                residualCapacity[u][v] -= pathFlow;
                if (!residualCapacity[v].ContainsKey(u))
                    residualCapacity[v][u] = 0;
                residualCapacity[v][u] += pathFlow;
                v = u;
            }
            augmentingPath.Add(source);
            augmentingPath.Reverse();

            maxFlow += pathFlow;

            OutputTextBox.AppendText($"Найден дополняющий путь: {string.Join(" -> ", augmentingPath)} с потоком {pathFlow}.\n\n");
            OutputTextBox.AppendText("Обновляем остаточные мощности.\n\n");
            HighlightPath(augmentingPath, graphData, Colors.Red);
            await Task.Delay(1000);
        }

        OutputTextBox.AppendText($"Максимальный поток из {source} в {sink} составляет {maxFlow}.\n");
        return maxFlow;
    }

    private void InitializeResidualNetwork(GraphData graphData)
    {
        residualCapacity = new Dictionary<string, Dictionary<string, double>>();

        foreach (var node in graphData.Nodes)
        {
            residualCapacity[node] = new Dictionary<string, double>();
            foreach (var (neighbor, capacity) in graphData.GetNeighbors(node))
            {
                residualCapacity[node][neighbor] = capacity;
            }

            var nodeGrid = graphData.GetNodeGrid(node);
            if (nodeGrid != null && nodeGrid.Children[0] is Ellipse ellipse)
            {
                // Сохраняем оригинальный цвет узла
                if (!originalNodeColors.ContainsKey(node))
                {
                    originalNodeColors[node] = ellipse.Fill;
                }
            }
        }
    }

    private async Task<Dictionary<string, string>> FindAugmentingPath(GraphData graphData, string source, string sink)
    {
        ResetHighlights(graphData);

        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        var parent = new Dictionary<string, string>();

        queue.Enqueue(source);
        visited.Add(source);

        OutputTextBox.AppendText($"Ищем дополняющий путь из {source} в {sink}.\n");

        while (queue.Count > 0)
        {
            string u = queue.Dequeue();
            OutputTextBox.AppendText($"Обрабатываем узел {u}.\n");

            var uGrid = graphData.GetNodeGrid(u);
            if (uGrid != null) HighlightNode(uGrid, Colors.DarkOrange);
            await Task.Delay(500);

            foreach (var vPair in residualCapacity[u])
            {
                string v = vPair.Key;
                double capacity = vPair.Value;

                if (capacity > 0 && !visited.Contains(v))
                {
                    visited.Add(v);
                    parent[v] = u;

                    var edge = graphData.GetEdge(u, v);
                    if (edge != null) HighlightEdge(edge, Colors.DarkOrange);

                    await Task.Delay(500);

                    if (v == sink)
                    {
                        OutputTextBox.AppendText($"Найден путь до стока {sink}.\n");
                        return parent;
                    }

                    queue.Enqueue(v);
                }
            }
        }

        OutputTextBox.AppendText("Дополняющий путь не найден.\n");
        return null;
    }

    private void ResetHighlights(GraphData graphData)
    {
        foreach (var node in graphData.Nodes)
        {
            var nodeGrid = graphData.GetNodeGrid(node);
            if (nodeGrid != null && nodeGrid.Children[0] is Ellipse ellipse)
            {
                if (originalNodeColors.ContainsKey(node))
                {
                    ellipse.Fill = originalNodeColors[node];
                }
            }
        }

        foreach (var node in graphData.Nodes)
        {
            foreach (var (neighbor, _) in graphData.GetNeighbors(node))
            {
                var edge = graphData.GetEdge(node, neighbor);
                if (edge != null)
                {
                    edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2D32"));
                    edge.StrokeThickness = 5;
                }
            }
        }
    }

    private void HighlightEdge(Line edge, Color color)
    {
        edge.Stroke = new SolidColorBrush(color);
        edge.StrokeThickness = 5;
    }

    private void HighlightNode(Grid node, Color color)
    {
        if (node.Children[0] is Ellipse ellipse)
        {
            ellipse.Fill = new SolidColorBrush(color);
        }
    }

    private async void HighlightPath(List<string> path, GraphData graphData, Color highlightColor)
    {
        var originalColors = new Dictionary<string, Brush>();

        for (int i = 0; i < path.Count - 1; i++)
        {
            var edge = graphData.GetEdge(path[i], path[i + 1]);
            if (edge != null)
            {
                originalColors[$"{path[i]}->{path[i + 1]}"] = edge.Stroke;
                edge.Stroke = new SolidColorBrush(highlightColor);
                edge.StrokeThickness = 5;
            }

            var nodeGrid = graphData.GetNodeGrid(path[i]);
            if (nodeGrid != null && nodeGrid.Children[0] is Ellipse ellipse)
            {
                if (!originalColors.ContainsKey($"Node_{path[i]}"))
                {
                    originalColors[$"Node_{path[i]}"] = ellipse.Fill;
                }
                ellipse.Fill = new SolidColorBrush(highlightColor);
            }
        }

        var lastNode = path.Last();
        var lastNodeGrid = graphData.GetNodeGrid(lastNode);
        if (lastNodeGrid != null && lastNodeGrid.Children[0] is Ellipse lastEllipse)
        {
            if (!originalColors.ContainsKey($"Node_{lastNode}"))
            {
                originalColors[$"Node_{lastNode}"] = lastEllipse.Fill;
            }
            lastEllipse.Fill = new SolidColorBrush(highlightColor);
        }

        await Task.Delay(500);
        
        foreach (var key in originalColors.Keys)
        {
            if (key.StartsWith("Node_"))
            {
                string node = key.Substring(5);
                var nodeGrid = graphData.GetNodeGrid(node);
                if (nodeGrid != null && nodeGrid.Children[0] is Ellipse ellipse)
                {
                    ellipse.Fill = originalNodeColors.ContainsKey(node) ? originalNodeColors[node] : Brushes.Transparent;
                }
            }
            else
            {
                var edge = graphData.GetEdge(key.Split("->")[0], key.Split("->")[1]);
                if (edge != null)
                {
                    edge.Stroke = originalColors[key];
                    edge.StrokeThickness = 5;
                }
            }
        }
    }
}