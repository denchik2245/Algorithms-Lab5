using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5.Algorithms;

public class DepthFirstSearch
{
    private TextBox OutputTextBox;

    public DepthFirstSearch(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task Execute(GraphData graphData, string startNodeLabel)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<string>();
        var traversalOrder = new List<string>();

        stack.Push(startNodeLabel);
        OutputTextBox.AppendText($"Стек: [{startNodeLabel}]\n");

        while (stack.Count > 0)
        {
            string currentNode = stack.Pop();

            if (!visited.Contains(currentNode))
            {
                visited.Add(currentNode);
                traversalOrder.Add(currentNode);

                OutputTextBox.AppendText($"Стек: [{string.Join(", ", stack.Reverse())}]\n");

                var nodeGrid = graphData.GetNodeGrid(currentNode);
                if (nodeGrid != null)
                    HighlightNode(nodeGrid);

                var neighbors = graphData.GetNeighbors(currentNode)
                                          .Where(n => !visited.Contains(n.Item1))
                                          .Select(n => n.Item1)
                                          .ToList();

                if (neighbors.Count > 0)
                {
                    OutputTextBox.AppendText($"Смотрим на соседей {currentNode} — это {string.Join(", ", neighbors)}.\n");

                    foreach (var neighbor in neighbors)
                    {
                        stack.Push(neighbor);
                        var edge = graphData.GetEdge(currentNode, neighbor);
                        if (edge != null)
                            HighlightEdge(edge);
                    }

                    OutputTextBox.AppendText($"Добавляем их в стек. Стек: [{string.Join(", ", stack.Reverse())}]\n");
                }
                else
                {
                    OutputTextBox.AppendText($"У {currentNode} нет соседей.\n");
                }

                await Task.Delay(1000);
            }

            if (stack.Count > 0)
            {
                string nextNode = stack.Peek();
                OutputTextBox.AppendText($"\nБерём {nextNode}\n");
            }
        }

        OutputTextBox.AppendText($"Стек пуст, обход завершён.\n");

        OutputTextBox.AppendText($"Итоговый порядок обхода: {string.Join(", ", traversalOrder)}\n");
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