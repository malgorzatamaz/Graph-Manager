using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Graph_Manager.Model;
using Graph_Manager.View;
using PropertyChanged;

namespace Graph_Manager.ViewModel
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {
        public int MousePosX { get; set; }
        public int MousePosY { get; set; }
        public static int IdImage { get; set; }
        public static int IdEdge { get; set; }
        public static int IndexAction { get; set; }

        public string PathDeleteVertex { get; set; }
        public string PathDirectory { get; set; }
        public string PathRandom { get; set; }
        public string PathAddVertex { get; set; }
        public string PathMoveVertex { get; set; }
        public int IdState { get; set; }

        public Vertex Vertex { get; set; }
        public Edge Edge { get; set; }
        public Graph Graph { get; set; }
        public Graph GraphNew { get; set; }

        public string DegreeSequence { get; set; }
        public bool IsConnectedGraph { get; set; }
        public bool IsDragging { get; set; }

        public CompositeCollection ObjectCompositeCollection { get; set; }
        public ObservableCollection<Graph> GraphCollection { get; set; }

        public ICommand AddVertexCommand { get; set; }
        public ICommand MoveVertexCommand { get; set; }
        public ICommand DeleteVertexCommand { get; set; }
        public ICommand CanvasMouseLeftButtonDownCommand { get; set; }
        public ICommand OpenWindowRandomCommand { get; set; }
        public ICommand OpenWindowPruferCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand DragVertexCommand { get; set; }
        public ICommand FrontCommand { get; set; }
        public ICommand OpenWindowSaveCommand { get; set; }
        public ICommand OpenWindowLoadGraphCommand { get; set; }
        public ICommand CleanScreenCommand { get; set; }
        public ICommand MoveMouseCommand { get; set; }
        public bool IsLineSelectedRightButton
        {
            get
            {
                return Graph.Edges.Any(l => l.IsMouseLeftButtonDown == true);
            }
        }

        public bool IsImageSelectedRightButton
        {
            get
            {
                return Graph.Vertexes.Any(v => v.IsMouseRightButtonDown == true);
            }
        }

        public bool IsImageSelectedLeftButton
        {
            get
            {
                return Graph.Vertexes.Any(v => v.IsMouseLeftButtonDown == true);
            }
        }

        private bool AnySelected
        {
            get
            {
                return Graph.Vertexes.Any(v => v.Selected == true);
            }
        }

        public MainWindowViewModel()
        {
            Vertex = new Vertex();
            Graph = new Graph();
            IsConnectedGraph = true;
            GraphCollection = new ObservableCollection<Graph>();
            ObjectCompositeCollection = new CompositeCollection();
            PathDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            PathAddVertex = PathDirectory + Resources.AddSelected;
            PathDeleteVertex = PathDirectory + Resources.DeleteUnselected;
            PathMoveVertex = PathDirectory + Resources.DragUnselected;
            IdImage = IdEdge = 0;
            IndexAction = 1;
            AddVertexCommand = new RelayCommand(AddVertex, (n) => true);
            MoveVertexCommand = new RelayCommand(MoveVertex, (n) => true);
            DeleteVertexCommand = new RelayCommand(DeleteVertex, (n) => true);
            CanvasMouseLeftButtonDownCommand = new RelayCommand(CanvasMouseLeftButtonDown, (n) => true);
            OpenWindowRandomCommand = new RelayCommand(OpenWindowRandom, (n) => true);
            OpenWindowPruferCommand = new RelayCommand(OpenWindowPrufer, (n) => true);
            BackCommand = new RelayCommand(Back, (n) => IdState > 0 ? true : false);
            DragVertexCommand = new RelayCommand(DragVertex, (n) => false);
            FrontCommand = new RelayCommand(Front, (n) => true);
            OpenWindowSaveCommand = new RelayCommand(OpenWindowSaveToDatabase, (n) => true);
            OpenWindowLoadGraphCommand = new RelayCommand(OpenWindowLoadGraph, (n) => true);
            CleanScreenCommand = new RelayCommand(CleanScreen, (n) => true);
            MoveMouseCommand = new RelayCommand(MoveMouse, (n) => true);
        }

        private void MoveMouse(object obj)
        {
            MouseEventArgs mouseArgs = (MouseEventArgs)obj;
            Point newPosition = mouseArgs.GetPosition(obj as Canvas);
            MousePosX = (int)newPosition.X;
            MousePosY = (int)newPosition.Y;
        }

        private void CleanScreen(object obj)
        {
            Graph = new Graph();
            GraphCollection.Add(Graph);
            AddToObjectCompositeCollection();

        }

        private void Front(object obj)
        {
            if (GraphCollection.Count > IdState)
                IdState++;
            Graph = GraphCollection[IdState - 1];
            AddToObjectCompositeCollection();
        }

        private void Refresh()
        {
            IdImage = IdEdge = IdState = 0;
            Graph = new Graph();
            AddToObjectCompositeCollection();
        }

        private void Back(object obj)
        {
            if (IdState == 1)
            {
                Refresh();
            }
            else
            {
                Graph = GraphCollection[IdState - 2];
                AddToObjectCompositeCollection();
                IdState--;
            }
        }

        private void Save()
        {
            int count = GraphCollection.Count;
            if (IdState < GraphCollection.Count)
            {
                for (int i = 0; i < count - IdState; i++)
                {
                    GraphCollection.Remove(GraphCollection.Last());
                }

            }
            IdState++;
            GraphCollection.Add(new Graph
            {
                Vertexes = new ObservableCollection<Vertex>(),
                Edges = new ObservableCollection<Edge>()
            });
            foreach (var v in Graph.Vertexes)
            {
                GraphCollection.Last().Vertexes.Add(new Vertex
                {
                    IsMouseRightButtonDown = v.IsMouseRightButtonDown,
                    IsMouseLeftButtonDown = v.IsMouseLeftButtonDown,

                    IdVertex = v.IdVertex,
                    Position = v.Position,
                    Path = v.Path,
                    Margin = v.Margin,
                    ConnectedVertexes = new List<Vertex>(),
                    ConnectedEdges = new List<Edge>()
                });
                foreach (var conver in v.ConnectedVertexes)
                {
                    GraphCollection.Last().Vertexes.Last().ConnectedVertexes.Add(new Vertex
                    {
                        IsMouseRightButtonDown = conver.IsMouseRightButtonDown,
                        IsMouseLeftButtonDown = conver.IsMouseLeftButtonDown,
                        IdVertex = conver.IdVertex,
                        Position = conver.Position,
                        Path = conver.Path,
                        Margin = conver.Margin
                    });
                }
                foreach (var coned in v.ConnectedEdges)
                {
                    GraphCollection.Last().Edges.Add(new Edge
                    {
                        StartPoint = coned.StartPoint,
                        EndPoint = coned.EndPoint,
                        EndVertexId = coned.EndVertexId,
                        IdEdge = coned.IdEdge,
                        IsMouseLeftButtonDown = coned.IsMouseLeftButtonDown,
                        StartVertexId = coned.StartVertexId
                    });
                }
            }
        }

        private void OpenWindowLoadGraph(object obj)
        {
            Graph = new Graph();
            LoadGraphViewModel loadGraphViewModel = new LoadGraphViewModel(Graph);
            var winLoad = new LoadGraphWindow(loadGraphViewModel);
            loadGraphViewModel.Window = winLoad;
            winLoad.ShowDialog();
            if (loadGraphViewModel.ReadTo)
            {
                GraphCollection.Add(Graph);
                AddToObjectCompositeCollection();
            }

        }

        private void OpenWindowSaveToDatabase(object obj)
        {
            SaveToDatabaseViewModel saveViewModel = new SaveToDatabaseViewModel(Graph, DegreeSequence);
            var winSave = new SaveToDatabaseWindow(saveViewModel);
            saveViewModel.Window = winSave;
            winSave.ShowDialog();
        }

        private void OpenWindowPrufer(object obj)
        {
            Graph = new Graph();
            PruferWindowViewModel pruferViewModel = new PruferWindowViewModel(Graph, 400, 400);
            var winPrufer = new PruferWindow(pruferViewModel);
            pruferViewModel.Window = winPrufer;
            winPrufer.ShowDialog();

            if (pruferViewModel.ReadTo)
            {
                GraphCollection.Add(Graph);
                AddToObjectCompositeCollection();
            }
        }


        private void OpenWindowRandom(object obj)
        {
            Graph = new Graph();
            RandomWindowViewModel randomViewModel = new RandomWindowViewModel(Graph, 400, 400);
            var winRandom = new RandomWindow(randomViewModel);
            randomViewModel.Window = winRandom;
            winRandom.ShowDialog();
            if (randomViewModel.ReadTo)
            {
                GraphCollection.Add(Graph);
                AddToObjectCompositeCollection();
            }
        }

        private void CanvasMouseLeftButtonDown(object obj)
        {
            Point drawingPoint = Mouse.GetPosition(obj as Canvas);
            drawingPoint.Offset(-Convert.ToDouble(Resources.ImageWidth) / 2, -Convert.ToDouble(Resources.ImageHeight) / 2);

            //w przypadku kiedy zostal zaznaczony wierzcholek przy dodawaniu nowych (niemozliwe jest pozniej dodanie nowych wierzcholkow)
            if (IndexAction == 1 && AnySelected == false && IsImageSelectedLeftButton == true)
            {
                Vertex = Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true);
                Vertex.IsMouseLeftButtonDown = false;
            }

            //dodaje wolne wierzchołki
            else if (IndexAction == 1 && AnySelected == false && IsImageSelectedLeftButton == false)
            {
                Graph.Vertexes.Add(new Vertex()
                {
                    Position = drawingPoint,
                    IdVertex = IdImage,
                    Margin = new Thickness(drawingPoint.X, drawingPoint.Y, 0, 0)
                });
                IdImage++;
            }

            //dodaje wierzchołek połączony z aktualnie wybranymi (sprawdza czy nie kliknięto na inny wierzchołek,
            //jeśli tak to nie wykonuje operacji)tworze wierzchołek i linie
            else if (IndexAction == 1 && AnySelected == true && IsImageSelectedLeftButton == false)
            {
                Vertex = new Vertex()
                {
                    Position = drawingPoint,
                    IdVertex = IdImage,
                    Margin = new Thickness(drawingPoint.X, drawingPoint.Y, 0, 0)
                };
                IdImage++;
                Graph.Vertexes.Add(Vertex);
                AddEdge(Vertex);
            }

            //łaczy wybrane wierzchołki z innym który  jest zaznaczony
            else if (IndexAction == 1 && AnySelected == true && IsImageSelectedLeftButton == true)
            {
                Vertex = Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true);
                AddEdge(Vertex);
                Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true).IsMouseLeftButtonDown = false;
            }

            //usuwa dowolny wierzchołek
            else if (IndexAction == 3 && IsImageSelectedLeftButton == true)
            {
                Vertex = Graph.Vertexes.FirstOrDefault(v => v.IsMouseLeftButtonDown);
               // List<Edge> edgeList = Vertex.ConnectedEdges.;
                foreach (var edge in Vertex.ConnectedEdges.ToList())
                {
                    Graph.Edges.Remove(edge);
                    Edge = edge;
                    RemoveEdge();
                }
                Graph.Vertexes.Remove(Vertex);
            }

            //usuwa dowolną krawędź
            else if (IndexAction == 3 && IsLineSelectedRightButton == true)
            {
                Edge = Graph.Edges.FirstOrDefault(v => v.IsMouseLeftButtonDown == true);
                RemoveEdge();
                Graph.Edges.Remove(Edge);
            }

            //przesuwanie wierzcholkow
            else if (IndexAction == 2 && IsImageSelectedLeftButton == true)
            {
                Vertex = Graph.Vertexes.FirstOrDefault(v => v.IsMouseLeftButtonDown);
                DragVertexCommand = new RelayCommand(DragVertex, (n) => true);
            }

            AddToObjectCompositeCollection();
            Save();
        }

        private void RemoveEdge()
        {
            var startVertex = Graph.Vertexes.Where(v => v.IdVertex == Edge.StartVertexId).FirstOrDefault();
            var endVertex = Graph.Vertexes.Where(v => v.IdVertex == Edge.EndVertexId).FirstOrDefault();

            startVertex.ConnectedVertexes.Remove(endVertex);
            endVertex.ConnectedVertexes.Remove(startVertex);

            startVertex.ConnectedEdges.Remove(Edge);
            endVertex.ConnectedEdges.Remove(Edge);
        }

        private void DragVertex(object args)
        {
            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                IsDragging = true;
                MouseEventArgs mouseArgs = (MouseEventArgs)args;
                Point newPosition = mouseArgs.GetPosition(args as Canvas);
                newPosition.Offset(-Convert.ToDouble(Resources.ImageWidth) * 2.5, -Convert.ToDouble(Resources.ImageHeight) / 2);
                Vertex.Position = newPosition;

                var startEdges = Vertex.ConnectedEdges.Where(n => n.StartVertexId == Vertex.IdVertex).ToList();
                var endEdges = Vertex.ConnectedEdges.Where(n => n.EndVertexId == Vertex.IdVertex).ToList();
                foreach (var edge in startEdges)
                {
                    edge.StartPoint = newPosition;
                    edge.CalculateStartEndPoint(Graph);

                }

                foreach (var edge in endEdges)
                {
                    edge.EndPoint = newPosition;
                    edge.CalculateStartEndPoint(Graph);
                }

                Vertex.Margin = new Thickness(newPosition.X, newPosition.Y, 0, 0);

                Vertex.IsMouseLeftButtonDown = false;
            }
            else
            {
                IsDragging = false;
                DragVertexCommand = new RelayCommand(DragVertex, (n) => false);
            }


            AddToObjectCompositeCollection();
        }

        private void AddToObjectCompositeCollection()
        {
            ObjectCompositeCollection.Clear();
            ObjectCompositeCollection.Add(new CollectionContainer() { Collection = Graph.Vertexes });
            ObjectCompositeCollection.Add(new CollectionContainer() { Collection = Graph.Edges });

            if (!IsDragging)
                CreateDegreeSequence();
        }

        private void CreateDegreeSequence()
        {
            DegreeSequence = string.Empty;
            List<int> degreeSequence = new List<int>();
            foreach (var vertex in Graph.Vertexes)
                degreeSequence.Add(vertex.ConnectedVertexes.Count);

            CheckConnectivity();

            degreeSequence.Sort();
            DegreeSequence = string.Join(",", degreeSequence);
        }

        private void CheckConnectivity()
        {
            var listVertexes = Graph.Vertexes.Select(n => n.IdVertex).ToList();

            if (listVertexes.Count > 0)
            {
                List<int> stack = new List<int>();
                List<int> visitedVertexes = new List<int>();

                visitedVertexes.Add(listVertexes[0]);
                var vertex = Graph.Vertexes.FirstOrDefault(n => n.IdVertex == visitedVertexes[0]);
                listVertexes.Remove(visitedVertexes[0]);
                while (listVertexes.Count > 0)
                {
                    var tempStack = vertex.ConnectedVertexes.Select(n => n.IdVertex).ToList();
                    var sortedTempStack = tempStack.OrderByDescending(i => i);

                    foreach (var stackItem in sortedTempStack)
                    {
                        if (!visitedVertexes.Contains(stackItem) && !stack.Contains(stackItem))
                            stack.Add(stackItem);
                    }

                    if (stack.Any())
                    {
                        int lastVertex = stack.Last();
                        vertex = Graph.Vertexes.FirstOrDefault(n => n.IdVertex == lastVertex);
                        if (listVertexes.Contains(lastVertex))
                        {
                            listVertexes.Remove(lastVertex);
                            stack.Remove(lastVertex);
                            if (!visitedVertexes.Contains(lastVertex))
                                visitedVertexes.Add(lastVertex);
                        }
                    }
                    else
                        break;
                }
            }
            if (listVertexes.Any())
                IsConnectedGraph = false;
            else
                IsConnectedGraph = true;

        }

        private void AddEdge(Vertex vertex)
        {
            foreach (var item in Graph.Vertexes.Where(v => v.Selected == true).ToList())
            {
                if (vertex.IdVertex != item.IdVertex)
                {
                    bool isVertex = false;
                    if (item.ConnectedVertexes.Count > 0)
                    {
                        var ver = item.ConnectedVertexes.FirstOrDefault(n => n.IdVertex == vertex.IdVertex);
                        if (ver != null)
                            isVertex = true;
                    }

                    if (isVertex == false)
                    {
                        var edge = new Edge()
                        {
                            StartVertexId = item.IdVertex,
                            EndVertexId = vertex.IdVertex,
                            IdEdge = IdEdge
                        };
                        edge.CalculateStartEndPoint(Graph);
                        Graph.Edges.Add(edge);
                        IdEdge++;
                        item.ConnectedEdges.Add(edge);
                        item.ConnectedVertexes.Add(vertex);

                        vertex.ConnectedEdges.Add(edge);
                        vertex.ConnectedVertexes.Add(item);
                    }
                }
            }
        }


        private void DeleteVertex(object obj)
        {
            IndexAction = 3;
            PathAddVertex = PathDirectory + Resources.AddUnselected;
            PathDeleteVertex = PathDirectory + Resources.DeleteSelected;
            PathMoveVertex = PathDirectory + Resources.DragUnselected;
        }

        private void MoveVertex(object obj)
        {
            IndexAction = 2;
            PathAddVertex = PathDirectory + Resources.AddUnselected;
            PathDeleteVertex = PathDirectory + Resources.DeleteUnselected;
            PathMoveVertex = PathDirectory + Resources.DragSelected;
        }

        private void AddVertex(object obj) // 1-dodaj, 2 -move, 3- delete
        {
            IndexAction = 1;
            PathAddVertex = PathDirectory + Resources.AddSelected;
            PathDeleteVertex = PathDirectory + Resources.DeleteUnselected;
            PathMoveVertex = PathDirectory + Resources.DragUnselected;
        }
    }
}
