using System.Windows.Controls;

namespace Algorithms_Lab5._1task;

public class DepthFirstSearch
{
    private TextBox OutputTextBox;

    public DepthFirstSearch(TextBox outputTextBox)
    {
        OutputTextBox = outputTextBox;
    }

    public async Task Execute(Dictionary<int, List<(int, int)>> graph, int startNode, Action<int> highlightNode, Action<int, int> highlightEdge)
    {
        var visited = new HashSet<int>();
        await DFS(graph, startNode, visited, highlightNode, highlightEdge);
        OutputTextBox.AppendText("Обход в глубину завершён.\n");
    }

    private async Task DFS(Dictionary<int, List<(int, int)>> graph, int currentNode, HashSet<int> visited, Action<int> highlightNode, Action<int, int> highlightEdge)
    {
        visited.Add(currentNode);
        highlightNode(currentNode);
        OutputTextBox.AppendText($"Посещаем узел {currentNode}.\n");

        foreach (var (neighbor, weight) in graph[currentNode])
        {
            if (!visited.Contains(neighbor))
            {
                highlightEdge(currentNode, neighbor);
                OutputTextBox.AppendText($"Переходим к узлу {neighbor}. Ребро ({currentNode} -> {neighbor}) с весом {weight} подсвечено.\n");
                await Task.Delay(1000); // Для визуализации
                await DFS(graph, neighbor, visited, highlightNode, highlightEdge);
            }
        }
    }
}