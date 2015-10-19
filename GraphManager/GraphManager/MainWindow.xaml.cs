using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphManager.Model;
using MahApps.Metro.Controls;

namespace GraphManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private Graph Graph;
        private bool _selected = false;
        private int _selectedIndex;
        private Image _selectedImage;

        public MainWindow()
        {
            PreviewKeyDown += HandleEsc;

            Graph = new Graph();
            InitializeComponent();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
            {
                string localPath = AppDomain.CurrentDomain.BaseDirectory;
                BitmapImage source = new BitmapImage();

                _selected = false;

                source.BeginInit();
                source.UriSource = new Uri(localPath + "Circle_Blue.png", UriKind.Absolute);
                source.EndInit();
                _selectedImage.Source = source;
            }

        }

        private void DrawVertex(Vertex v)
        {
            string localPath = AppDomain.CurrentDomain.BaseDirectory;
            Image img = new Image();
            img.Width = 30;
            img.Height = 30;

            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(localPath + "Circle_Blue.png", UriKind.Absolute);
            source.EndInit();
            img.Source = source;


            Canvas.SetTop(img, v.Y);
            Canvas.SetLeft(img, v.X);

            Canvas.Children.Add(img);
            v.CanvasIndex = Canvas.Children.IndexOf(img);
        }

        private void JoinVertexes(Vertex vertex1, Vertex vertex2)
        {
            DrawEdge(vertex1, vertex2);
            vertex1.Neibourghs.Add(vertex2);
            vertex2.Neibourghs.Add(vertex1);

            Image img = Canvas.Children[vertex1.CanvasIndex] as Image;
            Canvas.Children.Remove(img);
            Canvas.Children.Add(img);
            _selectedIndex = Canvas.Children.Count - 1;
            vertex1.CanvasIndex = _selectedIndex;

            img = Canvas.Children[vertex2.CanvasIndex] as Image;
            Canvas.Children.Remove(img);
            Canvas.Children.Add(img);

        }

        private void AddVertexToGraph(Vertex addToVertex)
        {
            Point p = Mouse.GetPosition(Canvas); // pozycja myszy
            int vertexesCount = Graph.Vertexes.Count;
            Vertex newVertex =
                new Vertex
                {
                    X = Convert.ToInt32(p.X),
                    Y = Convert.ToInt32(p.Y),
                    Index = vertexesCount,
                };

            if (Graph.Vertexes.Count >= 1)
            {
                DrawEdge(addToVertex, newVertex);

                Canvas.Children.Remove(_selectedImage);
                // usuwanie i rysowanie wierzchołka na nowy, żeby był widoczy nad liną
                Canvas.Children.Add(_selectedImage);
                _selectedIndex = Canvas.Children.Count - 1;
                addToVertex.CanvasIndex = _selectedIndex;
            }

            Graph.Vertexes.Add(newVertex);

            addToVertex.Neibourghs.Add(newVertex); // Dodajemy sąsiadów każdemu wierzchołkowi, do użycia w algorytmach
            newVertex.Neibourghs.Add(addToVertex);

            DrawVertex(Graph.Vertexes[vertexesCount]);
        }

        private void DrawEdge(Vertex vertex1, Vertex vertex2)
        {
            int index;
            Line line = new Line();
            line.Stroke = Brushes.LightSteelBlue;

            line.X1 = vertex1.X + 15;
            line.Y1 = vertex1.Y + 15;

            line.X2 = vertex2.X + 15;
            line.Y2 = vertex2.Y + 15;

            line.StrokeThickness = 5;
            Canvas.Children.Add(line);
            index = Canvas.Children.IndexOf(line);

            vertex1.EdgesIndex.Add(index);
            //Dodajemy numer indeksu lini na canvasie, żeby potem je móc łatwo odnaleźć i usunąć
            vertex2.EdgesIndex.Add(index);
        }

        private void CanvasClear()
        {
            Canvas.Children.Clear();
            Graph.Vertexes.Clear();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CanvasClear();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            string localPath = AppDomain.CurrentDomain.BaseDirectory;

            if (e.OriginalSource is Image && !_selected)
            {
                Image originalImage = e.OriginalSource as Image;

                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(localPath + "Circle_Orange.png", UriKind.Absolute);
                source.EndInit();
                originalImage.Source = source;

                _selectedImage = originalImage;
                _selected = true;
                _selectedIndex = Canvas.Children.IndexOf(originalImage);

                return;
            }


            else if (e.OriginalSource is Image && _selected)
            {
                Image originalImage = e.OriginalSource as Image;

                if (CbxMode.SelectedIndex == 1)
                {
                    int deleteindex = Graph.GetIndex(_selectedIndex);

                    foreach (
                        int x in
                            Graph.Vertexes.Where(n => n.CanvasIndex == _selectedIndex).SelectMany(v => v.EdgesIndex))
                    {
                        Canvas.Children.RemoveAt(x);
                    }
                    Canvas.Children.Remove(originalImage);
                    Graph.Vertexes.RemoveAt(deleteindex);
                }
            }

            if (_selected && CbxMode.SelectedIndex == 0)
            {
                if (e.OriginalSource is Image)
                {
                    Image img = e.OriginalSource as Image;
                    int index = Canvas.Children.IndexOf(img);
                    Vertex v1 = Graph.Vertexes[Graph.GetIndex(_selectedIndex)];
                    Vertex v2 = Graph.Vertexes[Graph.GetIndex(index)];
                    JoinVertexes(v1, v2);
                }
                else
                {
                    int index = Graph.GetIndex(_selectedIndex);
                    AddVertexToGraph(Graph.Vertexes[index]);
                }
            }
            else if (!_selected && CbxMode.SelectedIndex == 0)
            {
                Point p = Mouse.GetPosition(Canvas);
                Vertex v = new Vertex
                {
                    X = Convert.ToInt32(p.X),
                    Y = Convert.ToInt32(p.Y),
                    Index = Graph.Vertexes.Count,
                };
                DrawVertex(v);
                Graph.Vertexes.Add(v);
            }
        }

    }
}
