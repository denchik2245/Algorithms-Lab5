using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

public class FordFulkerson
{
    private TextBox OutputTextBox;
    private Dictionary<string, Dictionary<string, double>> residualCapacity;

    public FordFulkerson(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task<double> Execute(GraphData graphData, string source, string sink)
    {
        // Инициализация резидуальной сети
        InitializeResidualNetwork(graphData);

        double maxFlow = 0;

        OutputTextBox.AppendText($"Поиск максимального потока из {source} в {sink}.\n");

        // Пока существует увеличивающий поток путь
        while (true)
        {
            // Ищем путь в резидуальной сети из source в sink
            var parent = await FindAugmentingPath(graphData, source, sink);
            if (parent == null)
            {
                // Путь не найден
                break;
            }

            // Находим минимальную пропускную способность на найденном пути
            double pathFlow = double.PositiveInfinity;
            string v = sink;
            while (v != source)
            {
                string u = parent[v];
                pathFlow = Math.Min(pathFlow, residualCapacity[u][v]);
                v = u;
            }

            // Обновляем резидуальные ёмкости
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

            OutputTextBox.AppendText($"Найден дополняющий путь: {string.Join(" -> ", augmentingPath)} с потоком {pathFlow}.\n");
            HighlightPath(augmentingPath, graphData, Colors.Orange);
            await Task.Delay(1000);
        }

        OutputTextBox.AppendText($"Максимальный поток: {maxFlow}\n");
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
                // Инициализируем резидуальную ёмкость равной пропускной способности исходного ребра
                residualCapacity[node][neighbor] = capacity;
            }
        }
    }

    private async Task<Dictionary<string, string>> FindAugmentingPath(GraphData graphData, string source, string sink)
    {
        // Поиск в ширину по резидуальной сети
        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        var parent = new Dictionary<string, string>();

        queue.Enqueue(source);
        visited.Add(source);

        OutputTextBox.AppendText($"Поиск дополняющего пути из {source} в {sink}\n");

        while (queue.Count > 0)
        {
            string u = queue.Dequeue();
            OutputTextBox.AppendText($"Обработка узла {u}\n");
            var uGrid = graphData.GetNodeGrid(u);
            if (uGrid != null) HighlightNode(uGrid, Colors.Yellow);
            await Task.Delay(500);

            foreach (var vPair in residualCapacity[u])
            {
                string v = vPair.Key;
                double capacity = vPair.Value;

                if (capacity > 0 && !visited.Contains(v))
                {
                    visited.Add(v);
                    parent[v] = u;

                    // Подсветка ребра (u, v)
                    var edge = graphData.GetEdge(u, v);
                    if (edge != null) HighlightEdge(edge, Colors.Yellow);

                    await Task.Delay(500);

                    if (v == sink)
                    {
                        // Цель достигнута
                        OutputTextBox.AppendText("Найден путь до стока.\n");
                        return parent;
                    }

                    queue.Enqueue(v);
                }
            }
        }

        OutputTextBox.AppendText("Дополняющий путь не найден.\n");
        return null;
    }

    // Подсветка рёбер
    private void HighlightEdge(Line edge, Color color)
    {
        edge.Stroke = new SolidColorBrush(color);
        edge.StrokeThickness = 5;
    }

    // Подсветка узлов
    private void HighlightNode(Grid node, Color color)
    {
        if (node.Children[0] is Ellipse ellipse)
        {
            ellipse.Fill = new SolidColorBrush(color);
        }
    }

    // Подсветка найденного дополняющего пути
    private void HighlightPath(List<string> path, GraphData graphData, Color color)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var edge = graphData.GetEdge(path[i], path[i + 1]);
            if (edge != null)
            {
                edge.Stroke = new SolidColorBrush(color);
                edge.StrokeThickness = 5;
            }

            // Подсветка узлов пути
            var nodeGrid = graphData.GetNodeGrid(path[i]);
            if (nodeGrid != null)
            {
                if (nodeGrid.Children[0] is Ellipse ellipse)
                {
                    ellipse.Fill = new SolidColorBrush(color);
                }
            }
        }

        // Подсветка последнего узла
        var lastNodeGrid = graphData.GetNodeGrid(path.Last());
        if (lastNodeGrid != null)
        {
            if (lastNodeGrid.Children[0] is Ellipse ellipse)
            {
                ellipse.Fill = new SolidColorBrush(color);
            }
        }
    }
}
