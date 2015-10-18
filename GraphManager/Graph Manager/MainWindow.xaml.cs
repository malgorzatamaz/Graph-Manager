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

        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point point = Mouse.GetPosition(Canvas);

            if (cbxMode.SelectedIndex == 0 && AnySelected == false)
            {
                Graph.Vertexes.Add(new Vertex()
                {
                    Position = point
                });
            }
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
                        EndPoint = point
                    };

                    Graph.Edges.Add(edge);

                    v.ConnectedEdges.Add(edge);
                    v.ConnectedVertexes.Add(vertex);

                    vertex.ConnectedEdges.Add(edge);
                    vertex.ConnectedVertexes.Add(v);
                });
            }
            else if (cbxMode.SelectedIndex == 0 && AnySelected == true && e.OriginalSource is Image)
            {
                int index = Canvas.Children.IndexOf(e.OriginalSource as Image);
                var vertex = Graph.Vertexes.First(v => v.Index == index);

                Graph.Vertexes.Where(v => v.Selected == true).ToList().ForEach(v =>
                {
                    var edge = new Edge()
                    {
                        StartPoint = v.Position,
                        EndPoint = vertex.Position
                    };

                    Graph.Edges.Add(edge);

                    v.ConnectedEdges.Add(edge);
                    v.ConnectedVertexes.Add(vertex);

                    vertex.ConnectedEdges.Add(edge);
                    vertex.ConnectedVertexes.Add(v);
                });
            }
            else if (cbxMode.SelectedIndex == 1 && e.OriginalSource is Image)
            {
                int index = Canvas.Children.IndexOf(e.OriginalSource as Image);
                var vertex = Graph.Vertexes.First(v => v.Index == index);

                vertex.ConnectedEdges.ForEach(m =>
                {
                    Graph.Edges.RemoveAll(v => v.Index == m.Index);
                });

                Graph.Vertexes.Remove(vertex);
            }

            Draw();
        }

        private void Draw()
        {
            Canvas.Children.Clear();

            //Edges draw
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

            //Vertex draw
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
    }
}
