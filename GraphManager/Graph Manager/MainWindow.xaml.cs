using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Graph_Manager.Model;

namespace Graph_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int ImageHeight = 30;
        private const int ImageWidth = 30;

        public MainWindow()
        {
            Graph = new Graph();
            InitializeComponent();
        }

        public string LocalPath => AppDomain.CurrentDomain.BaseDirectory;
        public bool AnySelected => Graph.Vertexes.Any(v => v.Selected == true);
        public Graph Graph { get; set; }

        //odpowiada za zaznaczanie wierzchołków
        private void Canvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Image)
            {
                int index = Canvas.Children.IndexOf(e.OriginalSource as Image);
                var vertex = Graph.Vertexes.First(v => v.Index == index);
                if (vertex.Selected == false)
                {
                    vertex.Selected = true;
                }
                else
                {
                    vertex.Selected = false;
                }
            }
            else
            {
                Graph.Vertexes.ForEach(v =>
                {
                    v.Selected = false;
                });
            }

            Draw();
        }

        //wykonuje operacje wybrane w menu
        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point point = Mouse.GetPosition(Canvas);

            //dodaje wolne wierzchołki
            if (cbxMode.SelectedIndex == 0 && AnySelected == false)
            {
                Graph.Vertexes.Add(new Vertex()
                {
                    Position = point
                });
            }
            //dodaje wierzchołek połączony z aktualnie wybranymi (sprawdza czy nie kliknięto na inny wierzchołek,
            //jeśli tak to nie wykonuje operacji)
            else if (cbxMode.SelectedIndex == 0 && AnySelected == true && !(e.OriginalSource is Image))
            {
                var vertex = new Vertex()
                {
                    Position = point
                };
                Graph.Vertexes.Add(vertex);

                Graph.Vertexes.Where(v => v.Selected == true).ToList().ForEach(v =>
                {
                    var edge = new Edge()
                    {
                        StartPoint = v.Position,
                        EndPoint = point,
                        StartVertex = v,
                        EndVertex = vertex
                    };

                    Graph.Edges.Add(edge);

                    v.ConnectedEdges.Add(edge);
                    v.ConnectedVertexes.Add(vertex);

                    vertex.ConnectedEdges.Add(edge);
                    vertex.ConnectedVertexes.Add(v);
                });
            }
            //łaczy wybrane wierzchołki z innym który nie jest zaznaczony
            else if (cbxMode.SelectedIndex == 0 && AnySelected == true && e.OriginalSource is Image)
            {
                int index = Canvas.Children.IndexOf(e.OriginalSource as Image);
                var vertex = Graph.Vertexes.First(v => v.Index == index);

                Graph.Vertexes.Where(v => v.Selected == true).ToList().ForEach(v =>
                {
                    var edge = new Edge()
                    {
                        StartPoint = v.Position,
                        EndPoint = vertex.Position,
                        StartVertex = v,
                        EndVertex = vertex
                    };

                    Graph.Edges.Add(edge);

                    v.ConnectedEdges.Add(edge);
                    v.ConnectedVertexes.Add(vertex);

                    vertex.ConnectedEdges.Add(edge);
                    vertex.ConnectedVertexes.Add(v);
                });
            }
            //usuwa dowolny wierzchołek
            else if (cbxMode.SelectedIndex == 1 && e.OriginalSource is Image)
            {
                int index = Canvas.Children.IndexOf(e.OriginalSource as Image);
                var vertex = Graph.Vertexes.FirstOrDefault(v => v.Index == index);

                vertex.ConnectedEdges.ForEach(m =>
                {
                    Graph.Edges.Remove(m);
                });

                Graph.Vertexes.Remove(vertex);
            }
            //usuwa dowolną krawędź
            else if (cbxMode.SelectedIndex == 1 && e.OriginalSource is Line)
            {
                int index = Canvas.Children.IndexOf(e.OriginalSource as Line);
                var edge = Graph.Edges.FirstOrDefault(v => v.Index == index);

                Graph.Edges.Remove(edge);
            }

            Draw();
        }

        private void Draw()
        {
            Canvas.Children.Clear();

            //rysuje krawędzie
            foreach (var edge in Graph.Edges)
            {
                Line line = new Line();
                line.Stroke = Brushes.LightSteelBlue;

                line.X1 = edge.StartPoint.X;
                line.Y1 = edge.StartPoint.Y;

                line.X2 = edge.EndPoint.X;
                line.Y2 = edge.EndPoint.Y;

                line.StrokeThickness = 5;
                Canvas.Children.Add(line);
                edge.Index = Canvas.Children.IndexOf(line);
            }

            //rysuje wierzchołki
            foreach (var vertex in Graph.Vertexes)
            {
                Image image = new Image();
                image.Width = ImageWidth;
                image.Height = ImageHeight;

                BitmapImage source = new BitmapImage();
                source.BeginInit();
                if (vertex.Selected == false)
                {
                    source.UriSource = new Uri(LocalPath + "Circle_Blue.png", UriKind.Absolute);
                }
                else
                {
                    source.UriSource = new Uri(LocalPath + "Circle_Orange.png", UriKind.Absolute);
                }
                source.EndInit();
                image.Source = source;

                Canvas.SetTop(image, vertex.Position.Y - ImageWidth / 2);
                Canvas.SetLeft(image, vertex.Position.X - ImageHeight / 2);
                Canvas.Children.Add(image);

                vertex.Index = Canvas.Children.IndexOf(image);
            }
        }

        private void cbxClear_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            Graph = new Graph();
        }
    }
}
