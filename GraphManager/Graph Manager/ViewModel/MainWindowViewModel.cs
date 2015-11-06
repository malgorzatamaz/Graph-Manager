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
        public Vertex Vertex { get; set; }
        public Dictionary<int, List<int>> VertexesMatrix { get; set; }
        public Graph Graph { get; set; }
        public Graph GraphNew { get; set; }
        public string PathDeleteVertex { get; set; }
        public string PathDirectory { get; set; }
        public string PathRandom { get; set; }
        public string PathAddVertex { get; set; }
        public string PathMoveVertex { get; set; }
        public static int IdImage { get; set; }
        public static int IdEdge { get; set; }
        public int IndexAction { get; set; }
        public CompositeCollection ObjectCompositeCollection { get; set; }
        public ObservableCollection<Graph> GraphCollection { get; set; }
        public ICommand AddVertexCommand { get; set; }
        public ICommand MoveVertexCommand { get; set; }
        public ICommand DeleteVertexCommand { get; set; }
        public ICommand CanvasMouseLeftButtonDownCommand { get; set; }
        public ICommand OpenWindowRandomCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand DragVertexCommand { get; set; }

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
            VertexesMatrix = new Dictionary<int, List<int>>();
            Graph = new Graph();
            GraphCollection=new ObservableCollection<Graph>();
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
            OpenWindowRandomCommand=new RelayCommand(OpenWindowRandom, (n)=>true);
            BackCommand=new RelayCommand(Back,(n)=>GraphCollection.Count>0?true:false);
            DragVertexCommand = new RelayCommand(DragVertex, (n) => false);
        }

        private void Back(object obj)
        {
            Graph = GraphCollection[GraphCollection.Count - 2];
            GraphCollection.Remove(GraphCollection.Last());
            AddToObjectCompositeCollection();
        }

        private void Save()
        {
            GraphCollection.Add(new Graph
            {
                Vertexes = new ObservableCollection<Vertex> (),
                Edges = new ObservableCollection<Edge>()
            });
            foreach (var v in Graph.Vertexes)
            {
                GraphCollection.Last().Vertexes.Add(new Vertex
                {
                    IsMouseRightButtonDown=v.IsMouseRightButtonDown,
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
                       EndVertex = coned.EndVertex,
                       IdEdge = coned.IdEdge,
                       IsMouseLeftButtonDown = coned.IsMouseLeftButtonDown,
                       StartVertex = coned.StartVertex
                    });
                }
            }
        }

        private void OpenWindowRandom(object obj)
        {
            GraphNew = new Graph();
            RandomViewModel randomViewModel = new RandomViewModel(GraphNew, 400, 400);        
            var winRandom = new RandomWindow(randomViewModel);
            winRandom.ShowDialog();
            if (randomViewModel.ReadTo)
            {
                Graph = GraphNew;
            }
        }

        private void CanvasMouseLeftButtonDown(object obj)
        {
            Point vertexCenterPoint = Mouse.GetPosition(obj as Canvas);
            Point drawingPoint = Mouse.GetPosition(obj as Canvas);
            drawingPoint.Offset(-Convert.ToDouble(Resources.ImageWidth) / 2, -Convert.ToDouble(Resources.ImageHeight) / 2);

            //dodaje wolne wierzchołki
            if (IndexAction == 1 && AnySelected == false && IsImageSelectedLeftButton == false)
            {
                Graph.Vertexes.Add(new Vertex()
                {
                    Position = vertexCenterPoint,
                    IdVertex = IdImage,
                    Margin = new Thickness(drawingPoint.X, drawingPoint.Y, 0, 0)       
                });
                IdImage++;
            }

            //dodaje wierzchołek połączony z aktualnie wybranymi (sprawdza czy nie kliknięto na inny wierzchołek,
            //jeśli tak to nie wykonuje operacji)

            else if (IndexAction == 1 && AnySelected == true && IsImageSelectedLeftButton == false)
            {
                Vertex = new Vertex()
                {
                    Position = vertexCenterPoint,
                    IdVertex = IdImage,
                    Margin = new Thickness(drawingPoint.X, drawingPoint.Y, 0, 0)
                };
                IdImage++;
                Graph.Vertexes.Add(Vertex);
                AddVertexToMatrix(Vertex);
                AddEdge(Vertex);
            }

            //łaczy wybrane wierzchołki z innym który nie jest zaznaczony
            else if (IndexAction == 1 && AnySelected == true && IsImageSelectedLeftButton == true)
            {
                Vertex = Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true);
                if(Vertex.ConnectedVertexes.Any()==false)
                    AddEdge(Vertex);
                else if (Vertex.ConnectedVertexes.FirstOrDefault(m => m.IdVertex == Vertex.IdVertex) == null)
                    AddEdge(Vertex);

                AddVertexToMatrix(Vertex);
                Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true).IsMouseLeftButtonDown = false;
            }

            //usuwa dowolny wierzchołek
            else if (IndexAction == 3 && IsImageSelectedLeftButton == true)
            {
                Vertex = Graph.Vertexes.FirstOrDefault(v => v.IsMouseLeftButtonDown);
                Vertex.ConnectedEdges.ForEach(m =>
                {
                    Graph.Edges.Remove(m);
                });
                Graph.Vertexes.Remove(Vertex);

                RemoveVertexFromMatrix(Vertex);
            }

            //usuwa dowolną krawędź
            else if (IndexAction == 3 && IsLineSelectedRightButton == true)
            {
                var edge = Graph.Edges.FirstOrDefault(v => v.IsMouseLeftButtonDown == true);

                VertexesMatrix[edge.StartVertex.IdVertex].Remove(edge.EndVertex.IdVertex);
                Graph.Edges.Remove(edge);
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

        private void DragVertex(object args)
        {
            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                MouseEventArgs mouseArgs = (MouseEventArgs)args;
                Point newPosition = mouseArgs.GetPosition(args as Canvas);
                newPosition.Offset(-Convert.ToDouble(Resources.ImageWidth) * 2, 0);
                Vertex.Position = newPosition;
                
                newPosition.Offset(-Convert.ToDouble(Resources.ImageWidth) /2, -Convert.ToDouble(Resources.ImageHeight) / 2);
                Vertex.Margin = new Thickness(newPosition.X, newPosition.Y, 0, 0);
                Vertex.IsMouseLeftButtonDown = false;
            }
            else
                DragVertexCommand = new RelayCommand(DragVertex, (n) => false);
        }

        private void RemoveVertexFromMatrix(Vertex vertex)
        {
            if (VertexesMatrix.ContainsKey(vertex.IdVertex))
                VertexesMatrix.Remove(vertex.IdVertex);

            foreach (var key in VertexesMatrix.Keys)
            {
                if (VertexesMatrix[key].Contains(vertex.IdVertex))
                    VertexesMatrix[key].Remove(vertex.IdVertex);

                if (VertexesMatrix[key].Count == 0)
                    VertexesMatrix.Remove(vertex.IdVertex);
            }
        }

        private void AddVertexToMatrix(Vertex vertex)
        {
            var selectedVertexes = Graph.Vertexes.Where(v => v.Selected == true);
            foreach (var selected in selectedVertexes)
            {
                if (!VertexesMatrix.ContainsKey(selected.IdVertex))
                    VertexesMatrix.Add(
                                        selected.IdVertex,
                                        new List<int> { vertex.IdVertex }
                                        );
                else
                    VertexesMatrix[selected.IdVertex].Add(vertex.IdVertex);
            }
            
            
        }

        private void AddToObjectCompositeCollection()
        {
            ObjectCompositeCollection.Clear();
            ObjectCompositeCollection.Add(new CollectionContainer() { Collection = Graph.Vertexes });
            ObjectCompositeCollection.Add(new CollectionContainer() { Collection = Graph.Edges });
           
        }

        private void AddEdge(Vertex vertex)
        {
            foreach (var item in Graph.Vertexes.Where(v => v.Selected == true).ToList())
            {
                var edge = new Edge()
                {
                    StartVertex = item,
                    EndVertex = vertex,
                    IdEdge = IdEdge
                };
                Graph.Edges.Add(edge);
                IdEdge++;
                item.ConnectedEdges.Add(edge);
                item.ConnectedVertexes.Add(vertex);

                vertex.ConnectedEdges.Add(edge);
                vertex.ConnectedVertexes.Add(item);
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
