using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Algorithms;

public class DijkstraAlgorithm
{
    private TextBox OutputTextBox;
    private string startNode;
    private string endNode;

    public DijkstraAlgorithm(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task Execute(GraphData graphData, string startNode, string endNode)
    {
        this.startNode = startNode;
        this.endNode = endNode;

        OutputTextBox.AppendText($"Ищем кратчайший путь от {startNode} до {endNode}.\n");

        var distances = new Dictionary<string, double>();
        var previousNodes = new Dictionary<string, string>();
        var unvisitedNodes = new HashSet<string>();
        
        foreach (var node in graphData.Nodes)
        {
            distances[node] = double.PositiveInfinity;
            previousNodes[node] = null;
            unvisitedNodes.Add(node);
        }

        distances[startNode] = 0;

        while (unvisitedNodes.Count > 0)
        {
            var currentNode = unvisitedNodes.OrderBy(node => distances[node]).First();
            unvisitedNodes.Remove(currentNode);
            
            if (currentNode == endNode)
            {
                break;
            }
            
            foreach (var (neighbor, weight) in graphData.GetNeighbors(currentNode))
            {
                if (unvisitedNodes.Contains(neighbor))
                {
                    var newDist = distances[currentNode] + weight;
                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        previousNodes[neighbor] = currentNode;
                    }
                }
            }
            
            var nodeGrid = graphData.GetNodeGrid(currentNode);
            if (nodeGrid != null && nodeGrid.Children[0] is Ellipse ellipse)
            {
                ellipse.Fill = new SolidColorBrush(Colors.LightGreen);
            }

            OutputTextBox.AppendText($"Посещаем узел {currentNode}. Текущее расстояние: {distances[currentNode]}\n");

            await Task.Delay(1000);
        }
        
        var path = new List<string>();
        var currentPathNode = endNode;
        while (currentPathNode != null)
        {
            path.Insert(0, currentPathNode);
            currentPathNode = previousNodes[currentPathNode];
        }

        if (path.Count == 1)
        {
            OutputTextBox.AppendText($"Нет пути от {startNode} до {endNode}.\n");
        }
        else
        {
            OutputTextBox.AppendText($"Кратчайший путь: {string.Join(" -> ", path)}.\n");
            
            for (int i = 0; i < path.Count - 1; i++)
            {
                var fromNode = path[i];
                var toNode = path[i + 1];
                var edge = graphData.GetEdge(fromNode, toNode);
                if (edge != null)
                    HighlightEdge(edge);
            }
        }

        OutputTextBox.AppendText("Алгоритм завершен.\n");
    }

    private void HighlightEdge(Line edge)
    {
        edge.Stroke = new SolidColorBrush(Colors.Blue);
        edge.StrokeThickness = 3;
    }
}