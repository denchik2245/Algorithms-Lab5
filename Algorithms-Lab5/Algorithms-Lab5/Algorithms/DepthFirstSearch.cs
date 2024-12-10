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

        stack.Push(startNodeLabel);
        OutputTextBox.AppendText($"Начинаем обход в глубину с узла {startNodeLabel}.\n");

        while (stack.Count > 0)
        {
            string currentNode = stack.Pop();

            if (!visited.Contains(currentNode))
            {
                visited.Add(currentNode);
                
                var nodeGrid = graphData.GetNodeGrid(currentNode);
                if (nodeGrid != null)
                    HighlightNode(nodeGrid);

                OutputTextBox.AppendText($"Посещаем узел {currentNode}.\n");

                foreach (var (neighbor, weight) in graphData.GetNeighbors(currentNode))
                {
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                        
                        var edge = graphData.GetEdge(currentNode, neighbor);
                        if (edge != null)
                            HighlightEdge(edge);

                        OutputTextBox.AppendText($"Добавляем узел {neighbor} в стек. Ребро ({currentNode} -> {neighbor}) подсвечено.\n");
                    }
                }

                await Task.Delay(1000);
            }
        }

        OutputTextBox.AppendText("Обход в глубину завершён.\n");
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
