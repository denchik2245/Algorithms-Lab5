using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Algorithms_Lab5.Tools;
using Algorithms_Lab5.Utils;

namespace Algorithms_Lab5
{
    public partial class PageTask1 : Page
    {
        private GraphManager _graphManager;
        private Move _moveTool;
        private LoadGraph _loadGraphTool;
        public GraphManager GraphManager => _graphManager;
        public Canvas MainCanvasControl => MainCanvas;
        

        public PageTask1()
        {
            InitializeComponent();
    
            // Создаем GraphManager
            _graphManager = new GraphManager();

            // Инициализируем инструмент перемещения
            _moveTool = new Move(_graphManager.GraphData); // Передаем GraphData в Move
            _moveTool.Initialize(MainCanvas); // Привязываем Canvas

            // Инициализируем инструмент загрузки графа
            _loadGraphTool = new LoadGraph(_graphManager);
        }
        
        // Активируем режимы
        // Перемещение
        public void ActivateMoveMode()
        {
            // Деактивируем все режимы GraphManager
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.RemoveEdgeTool.IsActive = false;

            // Активируем MoveTool
            _moveTool.IsActive = true;
        }

        // Добавить узел
        public void ActivateAddNodeMode()
        {
            // Активируем AddNode, деактивируем остальные из GraphManager и MoveTool
            _graphManager.AddNodeTool.IsActive = true;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.RemoveEdgeTool.IsActive = false;
            _moveTool.IsActive = false;
        }

        // Удалить узел
        public void ActivateRemoveNodeMode()
        {
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = true;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.RemoveEdgeTool.IsActive = false;
            _moveTool.IsActive = false;
        }

        // Добавить ребро
        public void ActivateAddEdgeMode()
        {
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = true;
            _graphManager.RemoveEdgeTool.IsActive = false;
            _moveTool.IsActive = false;
        }

        // Удалить ребро
        public void ActivateRemoveEdgeMode()
        {
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.RemoveEdgeTool.IsActive = true;
            _moveTool.IsActive = false;
        }

        // Загрузить граф
        public void LoadGraph()
        {
            _loadGraphTool.IsActive = true;
            _loadGraphTool.Load(MainCanvas);
            _loadGraphTool.IsActive = false;
        }

        // Обработчик клика по Canvas
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_moveTool.IsActive)
            {
                _moveTool.StartDrag(e);
            }
            else if (_graphManager.AddNodeTool.IsActive)
            {
                Point clickPosition = e.GetPosition(MainCanvas);
                _graphManager.AddNodeTool.Add(MainCanvas, clickPosition);
            }
            else if (_graphManager.RemoveNodeTool.IsActive)
            {
                _graphManager.RemoveNodeTool.Remove(MainCanvas, e);
            }
            else if (_graphManager.AddEdgeTool.IsActive)
            {
                if (e.OriginalSource is FrameworkElement clickedElement)
                {
                    // Здесь важно, что узел находится в Grid, а Grid.Tag содержит уникальную метку узла
                    _graphManager.AddEdgeTool.SelectNode(MainCanvas, clickedElement.Parent as UIElement);
                }
            }
            else if (_graphManager.RemoveEdgeTool.IsActive)
            {
                _graphManager.RemoveEdgeTool.Remove(MainCanvas, e);
            }
            else if (_loadGraphTool.IsActive)
            {
                // Если вдруг надо загрузить повторно по клику (возможно вы так не делаете, но на всякий случай)
                _loadGraphTool.Load(MainCanvas);
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_moveTool.IsActive)
            {
                _moveTool.Drag(e);
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_moveTool.IsActive)
            {
                _moveTool.EndDrag();
            }
        }
    }

}