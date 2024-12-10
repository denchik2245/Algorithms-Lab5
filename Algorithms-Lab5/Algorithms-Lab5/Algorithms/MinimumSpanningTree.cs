using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Algorithms;

public class MinimumSpanningTree
{
    private TextBox OutputTextBox;
    private string selectedStartNode;

    public MinimumSpanningTree(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public void SetSelectedStartNode(string node)
    {
        selectedStartNode = node;
    }

    public async Task Execute(GraphData graphData)
    {
        if (string.IsNullOrEmpty(selectedStartNode))
        {
            OutputTextBox.AppendText("Начальный узел не выбран.\n");
            return;
        }
        
        var mstEdges = new List<(string, string, double)>();
        var visited = new HashSet<string>();
        var priorityQueue = new PriorityQueue<(string, string, double), double>();

        visited.Add(selectedStartNode);
        
        OutputTextBox.AppendText($"Начинаем строить минимальное остовное дерево с узла {selectedStartNode}.\n");
        
        foreach (var (neighbor, weight) in graphData.GetNeighbors(selectedStartNode))
        {
            priorityQueue.Enqueue((selectedStartNode, neighbor, weight), weight);
        }
        
        while (priorityQueue.Count > 0)
        {
            var (fromNode, toNode, weight) = priorityQueue.Dequeue();

            if (visited.Contains(toNode)) continue;

            visited.Add(toNode);
            mstEdges.Add((fromNode, toNode, weight));

            var edge = graphData.GetEdge(fromNode, toNode);
            if (edge != null)
                HighlightEdge(edge);

            OutputTextBox.AppendText($"Добавляем ребро ({fromNode} -> {toNode}) с весом {weight} в остовное дерево.\n");

            foreach (var (neighbor, edgeWeight) in graphData.GetNeighbors(toNode))
            {
                if (!visited.Contains(neighbor))
                {
                    priorityQueue.Enqueue((toNode, neighbor, edgeWeight), edgeWeight);
                }
            }

            await Task.Delay(1000);
        }

        OutputTextBox.AppendText("Минимальное остовное дерево построено.\n");
    }

    private void HighlightEdge(Line edge)
    {
        edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
    }
}