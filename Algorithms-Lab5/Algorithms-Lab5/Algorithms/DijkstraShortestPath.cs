using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Algorithms;

public class DijkstraShortestPath
{
    private TextBox OutputTextBox;

    public DijkstraShortestPath(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task Execute(GraphData graphData, string startNodeLabel, string endNodeLabel)
    {
        var distances = new Dictionary<string, double>();
        var previous = new Dictionary<string, string>();
        var priorityQueue = new PriorityQueue<string, double>();
        
        foreach (var node in graphData.Nodes)
        {
            distances[node] = double.PositiveInfinity;
            previous[node] = null;
        }

        distances[startNodeLabel] = 0;
        priorityQueue.Enqueue(startNodeLabel, 0);

        OutputTextBox.AppendText($"Начинаем поиск кратчайшего пути с узла {startNodeLabel} к узлу {endNodeLabel}.\n");

        // Пошаговое выполнение алгоритма с подсветкой промежуточных узлов и рёбер
        while (priorityQueue.Count > 0)
        {
            string currentNode = priorityQueue.Dequeue();
            OutputTextBox.AppendText($"Текущий узел: {currentNode} с расстоянием {distances[currentNode]}.\n");

            if (currentNode == endNodeLabel)
            {
                OutputTextBox.AppendText("Целевой узел достигнут.\n");
                break;
            }

            foreach (var (neighbor, weight) in graphData.GetNeighbors(currentNode))
            {
                double altDistance = distances[currentNode] + weight;
                if (altDistance < distances[neighbor])
                {
                    distances[neighbor] = altDistance;
                    previous[neighbor] = currentNode;
                    priorityQueue.Enqueue(neighbor, altDistance);
                    OutputTextBox.AppendText($"Обновлено расстояние до {neighbor}: {altDistance} через {currentNode}.\n");
                }
                
                var edge = graphData.GetEdge(currentNode, neighbor);
                if (edge != null)
                {
                    HighlightEdge(edge);
                }
            }
            
            var nodeGrid = graphData.GetNodeGrid(currentNode);
            if (nodeGrid != null)
            {
                HighlightNode(nodeGrid);
            }
            
            await Task.Delay(1000);
        }

        // Восстановление пути
        var path = new List<string>();
        string temp = endNodeLabel;
        while (previous[temp] != null)
        {
            path.Add(temp);
            temp = previous[temp];
        }

        if (temp == startNodeLabel)
        {
            path.Add(startNodeLabel);
            path.Reverse();
            OutputTextBox.AppendText($"Найден путь: {string.Join(" -> ", path)}\n");
            HighlightPath(path, graphData);
        }
        else
        {
            OutputTextBox.AppendText("Путь не найден.\n");
        }

        OutputTextBox.AppendText($"Поиск кратчайшего пути завершён. Общее расстояние: {distances[endNodeLabel]}.\n");
    }

    // Подсветка рёбер
    private void HighlightEdge(Line edge)
    {
        edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C3CC47")); //Желтый цвет
        edge.StrokeThickness = 5;
    }

    // Подсветка узлов
    private void HighlightNode(Grid node)
    {
        if (node.Children[0] is Ellipse ellipse)
        {
            ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C3CC47")); //Желтый цвет
        }
    }

    // Подсветка найденного пути
    private void HighlightPath(List<string> path, GraphData graphData)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var edge = graphData.GetEdge(path[i], path[i + 1]);
            if (edge != null)
            {
                edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
                edge.StrokeThickness = 5;
            }

            // Подсветка узлов пути
            var nodeGrid = graphData.GetNodeGrid(path[i]);
            if (nodeGrid != null)
            {
                if (nodeGrid.Children[0] is Ellipse ellipse)
                {
                    ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
                }
            }
        }

        // Подсветка последнего узла
        var lastNodeGrid = graphData.GetNodeGrid(path.Last());
        if (lastNodeGrid != null)
        {
            if (lastNodeGrid.Children[0] is Ellipse ellipse)
            {
                ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
            }
        }
    }
}