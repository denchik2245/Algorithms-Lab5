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
        var orderOfVisit = new List<string>();

        queue.Enqueue(startNodeLabel);
        visited.Add(startNodeLabel);
        orderOfVisit.Add(startNodeLabel);

        while (queue.Count > 0)
        {
            string currentNode = queue.Dequeue();

            var nodeGrid = graphData.GetNodeGrid(currentNode);
            if (nodeGrid != null)
                HighlightNode(nodeGrid);

            OutputTextBox.AppendText("\n");
            OutputTextBox.AppendText($"Берём {currentNode} из очереди.\n");

            var neighborsToAdd = new List<string>();

            foreach (var (neighbor, weight) in graphData.GetNeighbors(currentNode))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    neighborsToAdd.Add(neighbor);

                    var edge = graphData.GetEdge(currentNode, neighbor);
                    if (edge != null)
                        HighlightEdge(edge);
                }
            }

            if (neighborsToAdd.Count > 0)
            {
                OutputTextBox.AppendText($"Добавляем соседей {string.Join(", ", neighborsToAdd)} в очередь.\n");
                foreach (var neighbor in neighborsToAdd)
                {
                    queue.Enqueue(neighbor);
                    orderOfVisit.Add(neighbor);
                }

                OutputTextBox.AppendText($"Очередь: [{string.Join(", ", queue)}]\n");
            }
            else
            {
                OutputTextBox.AppendText($"У узла {currentNode} нет соседей.\n");
            }

            await Task.Delay(1000);
        }

        OutputTextBox.AppendText("\nИтоговый порядок обхода: ");
        OutputTextBox.AppendText(string.Join(", ", orderOfVisit) + "\n");
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