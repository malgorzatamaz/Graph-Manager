using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;
using Graph_Manager.View;
using MoreLinq;

namespace Graph_Manager.ViewModel
{
    public class RandomWindowViewModel
    {
        public bool ReadTo { get; set; }
        private Graph _graph;
        private bool _onCircle;
        private int _canvasWidth;
        public RandomWindow Window { get; set; }
        private int _canvasHeight;
        private Regex _expression;
        public Action CloseAction { get; set; }

        public RandomWindowViewModel(Graph graph, int canvasWidth, int canvasHeight)
        {
            ReadTo = false;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _graph = graph;
            _expression = new Regex("[1-9]+([,]{1}[1-9])");
            OnCircle = true;
            RandomizeCommand = new RelayCommand(Randomize, Validation.IsEven);
        }

        public bool OnCircle
        {
            get { return _onCircle; }
            set { _onCircle = value; }
        }

        public ICommand RandomizeCommand { get; set; }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand((o =>
                {
                    ReadTo = false;
                    CloseAction();
                }));
            }
        }

        public void Randomize(object obj)
        {
            ReadTo = true;

            string sequenceString = (string)obj;
            List<int> degreeSequence = Validation.SplitSequence(sequenceString);

            int radius,
                vertexIndex,
                angleChange = 0,
                angle,
                sum = degreeSequence.Sum();

            Vertex maxEdgesVertex = new Vertex();
            Edge newEdge;
            Random random = new Random();
            Point p = new Point();
            angle = 360 / degreeSequence.Count;

            MainWindowViewModel.IdImage = 0;
            MainWindowViewModel.IdEdge = 0;

            for (int i = 0; i < degreeSequence.Count; i++)
            {
                if (OnCircle)
                {
                    radius = (_canvasHeight / 2) - 20;
                    p.X = 0.5 * _canvasWidth + (radius * Math.Sin(angleChange));
                    p.Y = 0.5 * _canvasHeight + (radius * Math.Cos(angleChange));
                    angleChange += angle;
                }
                else
                {
                    p.X = random.Next(10, _canvasWidth - 10);
                    p.Y = random.Next(10, _canvasHeight - 10);
                }

                _graph.Vertexes.Add(new Vertex
                {
                    Position = p,
                    Margin = new Thickness(p.X, p.Y, 0, 0),
                    IdVertex = MainWindowViewModel.IdImage
                });
                MainWindowViewModel.IdImage++;
            }

            for (int i = 0; i < _graph.Vertexes.Count; i++)
            {
                _graph.Vertexes[i].Degree = degreeSequence[i];
            }

            while (_graph.Vertexes.Sum(s => s.Degree) > 0)
            {
                Vertex maxVertex, radomVertex;

                do
                {
                    radomVertex = _graph.Vertexes[random.Next(_graph.Vertexes.Count)];
                } while (radomVertex.Degree == 0);


                try
                {
                    maxVertex = _graph.Vertexes
                       .Where(n => n.IdVertex != radomVertex.IdVertex && !n.ConnectedVertexes.Contains(radomVertex))
                       .MaxBy(n => n.Degree); //z dołączonej biblioteki morelinq
                }
                catch
                {
                    continue; //w razie gdyby sekwencja nic nie znalazła to losujemy dalej ;)
                }

                newEdge = new Edge
                {
                    StartVertexId = radomVertex.IdVertex,
                    EndVertexId = maxVertex.IdVertex,
                    IdEdge = MainWindowViewModel.IdEdge
                };

                MainWindowViewModel.IdEdge++;
                newEdge.CalculateStartEndPoint(_graph);
                _graph.Edges.Add(newEdge);

                radomVertex.ConnectedEdges.Add(newEdge);
                maxVertex.ConnectedEdges.Add(newEdge);

                radomVertex.ConnectedVertexes.Add(maxVertex);
                maxVertex.ConnectedVertexes.Add(radomVertex);

                radomVertex.Degree--;
                maxVertex.Degree--;
            }

            vertexIndex = _graph.Vertexes.Count;

            Window.Close();
        }
    }
}
