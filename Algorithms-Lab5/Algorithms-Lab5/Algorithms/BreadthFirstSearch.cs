using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Algorithms;

public class BreadthFirstSearch
{
    private TextBox OutputTextBox;

    public BreadthFirstSearch(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task Execute(GraphData graphData, string startNodeLabel)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<string>();

        queue.Enqueue(startNodeLabel);
        visited.Add(startNodeLabel);

        OutputTextBox.AppendText($"Начинаем обход в ширину с узла {startNodeLabel}.\n");

        while (queue.Count > 0)
        {
            string currentNode = queue.Dequeue();
            
            var nodeGrid = graphData.GetNodeGrid(currentNode);
            if (nodeGrid != null)
                HighlightNode(nodeGrid);

            OutputTextBox.AppendText($"Посещаем узел {currentNode}.\n");

            foreach (var (neighbor, weight) in graphData.GetNeighbors(currentNode))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    
                    var edge = graphData.GetEdge(currentNode, neighbor);
                    if (edge != null)
                        HighlightEdge(edge);

                    OutputTextBox.AppendText($"Добавляем узел {neighbor} в очередь. Ребро ({currentNode} -> {neighbor}) подсвечено.\n");
                }
            }

            await Task.Delay(1000);
        }

        OutputTextBox.AppendText("Обход в ширину завершён.\n");
    }

    private void HighlightNode(Grid node)
    {
        if (node.Children[0] is Ellipse ellipse)
        {
            ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
        }
    }

    private void HighlightEdge(Line edge)
    {
        edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
    }
}