using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Graph_Manager.Model;
using PropertyChanged;

namespace Graph_Manager.ViewModel
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {
        private bool _isImageSelectedLeftButton;
        private bool _isImageSelectedRightButton;
        private bool _isLineSelectedRightButton;
        public Graph Graph { get; set; }
        public string PathDeleteVertex { get; set; }
        public string PathAddVertex { get; set; }
        public string PathMoveVertex { get; set; }
        public static int IdImage { get; set; }
        public static int IdEdge { get; set; }
        public int IndexAction { get; set; }
        public CompositeCollection ObjectCompositeCollection { get; set; }
        public ICommand AddVertexCommand { get; set; }
        public ICommand MoveVertexCommand { get; set; }
        public ICommand DeleteVertexCommand { get; set; }
        public ICommand CanvasMouseLeftButtonDownCommand { get; set; }
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
            Graph = new Graph();
            ObjectCompositeCollection = new CompositeCollection();
            PathAddVertex = AppDomain.CurrentDomain.BaseDirectory + "AddSelected.png";
            PathDeleteVertex = AppDomain.CurrentDomain.BaseDirectory + "DeleteUnselected.png";
            PathMoveVertex = AppDomain.CurrentDomain.BaseDirectory + "DragUnselected.png";
            IdImage = IdEdge = 0;
            IndexAction = 1;
            AddVertexCommand = new RelayCommand(AddVertex, (n) => true);
            MoveVertexCommand = new RelayCommand(MoveVertex, (n) => true);
            DeleteVertexCommand = new RelayCommand(DeleteVertex, (n) => true);
            CanvasMouseLeftButtonDownCommand = new RelayCommand(CanvasMouseLeftButtonDown, (n) => true);
        }

        private void CanvasMouseLeftButtonDown(object obj)
        {
            Point point = Mouse.GetPosition(obj as Canvas);
            point.X = point.X - Convert.ToDouble(Resources.ImageWidth) / 2;
            point.Y = point.Y - Convert.ToDouble(Resources.ImageHeight) / 2;
            //dodaje wolne wierzchołki
            if (IndexAction == 1 && AnySelected == false && IsImageSelectedLeftButton == false)
            {
                Graph.Vertexes.Add(new Vertex()
                {
                    Position = point,
                    IdVertex = IdImage,
                    Margin = new Thickness(point.X, point.Y, 0, 0)
                });
                IdImage++;
                AddToObjectCompositeCollection();
            }

            //dodaje wierzchołek połączony z aktualnie wybranymi (sprawdza czy nie kliknięto na inny wierzchołek,
            //jeśli tak to nie wykonuje operacji)

            else if (IndexAction == 1 && AnySelected == true && IsImageSelectedLeftButton == false)
            {
                var vertex = new Vertex()
                {
                    Position = point,
                    IdVertex = IdImage,
                    Margin = new Thickness(point.X, point.Y, 0, 0)
                };
                IdImage++;
                Graph.Vertexes.Add(vertex);
                AddEdge(vertex);
            }

            //łaczy wybrane wierzchołki z innym który nie jest zaznaczony
            else if (IndexAction == 1 && AnySelected == true && IsImageSelectedLeftButton == true)
            {
                var vertex = Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true);
                if(vertex.ConnectedVertexes.Any()==false)
                    AddEdge(vertex);
                else if (Graph.Vertexes.First().ConnectedVertexes.First(m => m.IdVertex == vertex.IdVertex) == null)
                    AddEdge(vertex);

                Graph.Vertexes.First(v => v.IsMouseLeftButtonDown == true).IsMouseLeftButtonDown = false;
            }

            //usuwa dowolny wierzchołek
            else if (IndexAction == 3 && IsImageSelectedLeftButton == true)
            {
                var vertex = Graph.Vertexes.FirstOrDefault(v => v.IsMouseLeftButtonDown);
                vertex.ConnectedEdges.ForEach(m =>
                {
                    Graph.Edges.Remove(m);
                });
                Graph.Vertexes.Remove(vertex);
            }

            //usuwa dowolną krawędź
            else if (IndexAction == 3 && IsLineSelectedRightButton == true)
            {
                var edge = Graph.Edges.FirstOrDefault(v => v.IsMouseLeftButtonDown == true);
                Graph.Edges.Remove(edge);
            }
            AddToObjectCompositeCollection();
        }

        private void AddToObjectCompositeCollection()
        {
            ObjectCompositeCollection.Add(new CollectionContainer() { Collection = Graph.Vertexes });
            ObjectCompositeCollection.Add(new CollectionContainer() { Collection = Graph.Edges });
        }

        private void AddEdge(Vertex vertex)
        {
            foreach (var item in Graph.Vertexes.Where(v => v.Selected == true).ToList())
            {
                var edge = new Edge()
                {
                    StartPoint = item.Position,
                    EndPoint = vertex.Position,
                    StartVertex = item,
                    EndVertex = vertex,
                    IdEdge = IdEdge
                };
                CalculateStartEndPoint(edge);
                Graph.Edges.Add(edge);
                IdEdge++;
                item.ConnectedEdges.Add(edge);
                item.ConnectedVertexes.Add(vertex);

                vertex.ConnectedEdges.Add(edge);
                vertex.ConnectedVertexes.Add(item);
            }
        }
        /// <summary>
        /// sluzy do obcinania line, zeby linii nie bylo na obrazku + przemieszczenie do centru obrazka
        /// </summary>
        /// <param name="edge"></param>

        private void CalculateStartEndPoint(Edge edge)
        {
            double DiagonalBig = Math.Sqrt(Math.Pow(edge.EndPoint.X - edge.StartPoint.X, 2) + Math.Pow(edge.EndPoint.Y - edge.StartPoint.Y, 2));
            Point pointStart = new Point();
            Point pointEnd = new Point();
            pointStart.X = (edge.EndPoint.X - edge.StartPoint.X) / DiagonalBig * Convert.ToUInt32(Resources.Radius) + edge.StartPoint.X;
            pointStart.Y = (edge.EndPoint.Y - edge.StartPoint.Y) / DiagonalBig * Convert.ToUInt32(Resources.Radius) + edge.StartPoint.Y;
            pointEnd.X = edge.EndPoint.X - (edge.EndPoint.X - edge.StartPoint.X) / DiagonalBig * Convert.ToUInt32(Resources.Radius);
            pointEnd.Y = edge.EndPoint.Y - (edge.EndPoint.Y - edge.StartPoint.Y) / DiagonalBig * Convert.ToUInt32(Resources.Radius);
            pointStart.X = pointStart.X + Convert.ToDouble(Resources.ImageWidth) / 2;
            pointStart.Y = pointStart.Y + Convert.ToDouble(Resources.ImageHeight) / 2;
            pointEnd.X = pointEnd.X + Convert.ToDouble(Resources.ImageWidth) / 2;
            pointEnd.Y = pointEnd.Y + Convert.ToDouble(Resources.ImageHeight) / 2;

            edge.StartPoint = pointStart;
            edge.EndPoint = pointEnd;
        }

        private void DeleteVertex(object obj)
        {
            IndexAction = 3;
            PathAddVertex = AppDomain.CurrentDomain.BaseDirectory + "AddUnselected.png";
            PathDeleteVertex = AppDomain.CurrentDomain.BaseDirectory + "DeleteSelected.png";
            PathMoveVertex = AppDomain.CurrentDomain.BaseDirectory + "DragUnselected.png";
        }

        private void MoveVertex(object obj)
        {
            IndexAction = 2;
            PathAddVertex = AppDomain.CurrentDomain.BaseDirectory + "AddUnselected.png";
            PathDeleteVertex = AppDomain.CurrentDomain.BaseDirectory + "DeleteUnselected.png";
            PathMoveVertex = AppDomain.CurrentDomain.BaseDirectory + "DragSelected.png";
        }

        private void AddVertex(object obj) // 1-dodaj, 2 -move, 3- delete
        {
            IndexAction = 1;
            PathAddVertex = AppDomain.CurrentDomain.BaseDirectory + "AddSelected.png";
            PathDeleteVertex = AppDomain.CurrentDomain.BaseDirectory + "DeleteUnselected.png";
            PathMoveVertex = AppDomain.CurrentDomain.BaseDirectory + "DragUnselected.png";
        }
    }
}
