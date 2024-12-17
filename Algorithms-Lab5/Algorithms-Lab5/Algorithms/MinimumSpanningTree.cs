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
            OutputTextBox.AppendText("Ошибка: начальный узел не выбран. Пожалуйста, выберите узел для начала построения дерева.\n");
            return;
        }

        var mstEdges = new List<(string, string, double)>();
        var visited = new HashSet<string>();
        var priorityQueue = new PriorityQueue<(string, string, double), double>();

        visited.Add(selectedStartNode);
        
        OutputTextBox.AppendText("\nАлгоритм будет выбирать рёбра с наименьшими весами, чтобы соединить все узлы и минимизировать общую длину дерева.\n \n");

        foreach (var (neighbor, weight) in graphData.GetNeighbors(selectedStartNode))
        {
            priorityQueue.Enqueue((selectedStartNode, neighbor, weight), weight);
            OutputTextBox.AppendText($"Добавляем ребро ({selectedStartNode} -> {neighbor}) с весом {weight} в очередь.\n");
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
            
            OutputTextBox.AppendText($"Выбираем ребро ({fromNode} -> {toNode}) с весом {weight} для добавления в минимальное остовное дерево.\n");
            OutputTextBox.AppendText($"Это ребро имеет наименьший вес среди всех доступных рёбер, которые соединяют уже посещенные узлы с не посещёнными.\n \n");

            foreach (var (neighbor, edgeWeight) in graphData.GetNeighbors(toNode))
            {
                if (!visited.Contains(neighbor))
                {
                    priorityQueue.Enqueue((toNode, neighbor, edgeWeight), edgeWeight);
                    OutputTextBox.AppendText($"Добавляем ребро ({toNode} -> {neighbor}) с весом {edgeWeight} в очередь.\n");
                }
            }
            
            await Task.Delay(1000);
        }

        OutputTextBox.AppendText("\nМинимальное остовное дерево построено.\n \n");
        OutputTextBox.AppendText("Все узлы теперь соединены с минимальной суммой весов рёбер.\n");
    }

    private void HighlightEdge(Line edge)
    {
        edge.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC5747"));
    }
}