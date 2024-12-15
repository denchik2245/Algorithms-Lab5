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
            _graphManager = new GraphManager();
            _moveTool = new Move(_graphManager.GraphData);
            _moveTool.Initialize(MainCanvas);
            _loadGraphTool = new LoadGraph(_graphManager);
        }
        
        // Активируем режимы
        // Перемещение
        public void ActivateMoveMode()
        {
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.RemoveEdgeTool.IsActive = false;
            _moveTool.IsActive = true;
        }

        // Добавить узел
        public void ActivateAddNodeMode()
        {
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
            _graphManager.AddDirectedEdgeTool.IsActive = false;
            _moveTool.IsActive = false;
        }

        // Добавить направляющее ребро
        public void ActivateAddDirectedEdgeMode()
        {
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.AddDirectedEdgeTool.IsActive = true;
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
        
        public void NonActive()
        {
            _graphManager.AddNodeTool.IsActive = false;
            _graphManager.RemoveNodeTool.IsActive = false;
            _graphManager.AddEdgeTool.IsActive = false;
            _graphManager.AddDirectedEdgeTool.IsActive = false;
            _graphManager.RemoveEdgeTool.IsActive = false;
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
            // Перемещение узлов
            if (_moveTool.IsActive)
            {
                _moveTool.StartDrag(e);
            }
            
            // Добавление узлов
            else if (_graphManager.AddNodeTool.IsActive)
            {
                Point clickPosition = e.GetPosition(MainCanvas);
                _graphManager.AddNodeTool.Add(MainCanvas, clickPosition);
            }
            
            // Удаление узлов
            else if (_graphManager.RemoveNodeTool.IsActive)
            {
                _graphManager.RemoveNodeTool.Remove(MainCanvas, e);
            }
            
            // Добавление обычных рёбер  
            else if (_graphManager.AddEdgeTool.IsActive)
            {
                if (e.OriginalSource is FrameworkElement clickedElement)
                {
                    _graphManager.AddEdgeTool.SelectNode(MainCanvas, clickedElement.Parent as UIElement);
                }
            }
            
            // Добавление направленных рёбер
            else if (_graphManager.AddDirectedEdgeTool.IsActive)
            {
                if (e.OriginalSource is FrameworkElement clickedElement)
                {
                    _graphManager.AddDirectedEdgeTool.SelectNode(MainCanvas, clickedElement.Parent as UIElement);
                }
            }
            
            // Удаление рёбер
            else if (_graphManager.RemoveEdgeTool.IsActive)
            {
                _graphManager.RemoveEdgeTool.Remove(MainCanvas, e);
            }
            
            // Загрузка графа
            else if (_loadGraphTool.IsActive)
            {
                _loadGraphTool.Load(MainCanvas);
            }
            
            else if (sender is Grid nodeGrid && nodeGrid.Tag is string nodeLabel)
            {
                ((MainWindow)Application.Current.MainWindow).SelectNode(nodeLabel);
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