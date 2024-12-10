using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algorithms_Lab5.Utils;

public class  GraphData
{
    private Dictionary<string, Dictionary<string, double>> adjacencyList = new();
    private Dictionary<string, Grid> nodeGrids = new();
    private Dictionary<(string, string), (Line line, TextBlock weight)> edges = new();

    public void AddNode(string nodeLabel, Grid grid)
    {
        if (!adjacencyList.ContainsKey(nodeLabel))
        {
            adjacencyList[nodeLabel] = new Dictionary<string, double>();
            nodeGrids[nodeLabel] = grid;
        }
    }

    public void AddEdge(string from, string to, double weight, Line line, TextBlock weightTextBlock)
    {
        if (!adjacencyList.ContainsKey(from) || !adjacencyList.ContainsKey(to))
            throw new InvalidOperationException("Один из узлов не существует.");

        adjacencyList[from][to] = weight;
        adjacencyList[to][from] = weight;
        edges[(from, to)] = (line, weightTextBlock);
        edges[(to, from)] = (line, weightTextBlock);
    }

    public void RemoveNode(string nodeLabel)
    {
        if (adjacencyList.ContainsKey(nodeLabel))
        {
            foreach (var neighbor in adjacencyList[nodeLabel].Keys.ToList())
            {
                RemoveEdge(nodeLabel, neighbor);
            }
            adjacencyList.Remove(nodeLabel);
            nodeGrids.Remove(nodeLabel);
        }
    }

    public void RemoveEdge(string from, string to)
    {
        if (adjacencyList.ContainsKey(from))
        {
            adjacencyList[from].Remove(to);
        }

        if (adjacencyList.ContainsKey(to))
        {
            adjacencyList[to].Remove(from);
        }

        if (edges.TryGetValue((from, to), out var edge))
        {
            edges.Remove((from, to));
            edges.Remove((to, from));

            var canvas = VisualTreeHelper.GetParent(edge.line) as Canvas;
            if (canvas != null)
            {
                canvas.Children.Remove(edge.line);
                canvas.Children.Remove(edge.weight);
            }
        }
    }

    public IEnumerable<string> Nodes => adjacencyList.Keys;

    public IEnumerable<(string neighbor, double weight)> GetNeighbors(string node)
    {
        if (adjacencyList.TryGetValue(node, out var neighbors))
        {
            foreach (var n in neighbors)
                yield return (n.Key, n.Value);
        }
    }

    public void UpdateNodePosition(string nodeLabel, double x, double y)
    {
        if (nodeGrids.TryGetValue(nodeLabel, out Grid node))
        {
            Canvas.SetLeft(node, x - (node.ActualWidth / 2));
            Canvas.SetTop(node, y - (node.ActualHeight / 2));
        }
    }

    public Grid GetNodeGrid(string label)
    {
        return nodeGrids.TryGetValue(label, out var grid) ? grid : null;
    }

    public Line GetEdge(string from, string to)
    {
        return edges.TryGetValue((from, to), out var edge) ? edge.line : null;
    }

    public TextBlock GetEdgeWeight(string from, string to)
    {
        return edges.TryGetValue((from, to), out var edge) ? edge.weight : null;
    }
}
