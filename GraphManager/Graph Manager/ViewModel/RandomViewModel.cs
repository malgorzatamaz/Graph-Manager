using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    class RandomViewModel
    {
        public bool ReadTo { get; set; }
        private Graph _graph;
        private bool _onCircle;
        private int _canvasWidth;
        private int _canvasHeight;

        public RandomViewModel(Graph graph, int canvasWidth, int canvasHeight)
        {
            ReadTo = false;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _graph = graph;
            RandomizeGraph = new RelayCommand(Randomize, IsEven);
            CloseCommand = new RelayCommand(o => ((Window)o).Close());
        }

        public bool OnCircle
        {
            get { return _onCircle; }
            set { _onCircle = value; }
        }
        public ICommand RandomizeGraph { get; set; }
        public ICommand CloseCommand;
        public void Randomize(object obj)
        {
            ReadTo = true;
            //string sequenceString = (string)obj;
            //List<int> degreeSequence = SplitDegreeSequence(sequenceString);
            //int maxEdges = 0, radius, vertexIndex, angleChange = 0, angle, sum = degreeSequence.Sum();
            //Vertex maxEdgesVertex = new Vertex();
            //Random r = new Random();
            //Point p = new Point();
            //angle = 360 / _graph.Vertexes.Count;

            //foreach (var x in degreeSequence)
            //{

            //    if (OnCircle)
            //    {
            //        radius = (_canvasHeight/2) - 20;
            //        p.X = 0.5 * _canvasWidth + (radius * Math.Sin(angleChange));
            //        p.Y = 0.5 * _canvasHeight + (radius * Math.Cos(angleChange));
            //        angleChange += angle;
            //    }
            //    else
            //    {
            //        p.X = r.Next(10, _canvasWidth - 10);
            //        p.Y = r.Next(10, _canvasHeight - 10);
            //    }

            //    _graph.Vertexes.Add(new Vertex {Position = p});
            //}
            
            //while (sum > -1)
            //{
            //    for (int i = 0; i < _graph.Vertexes[i].ConnectedEdges.Count; i++)
            //    {
            //        if (_graph.Vertexes[i].ConnectedEdges.Count < degreeSequence[i])
            //        {
            //            foreach (var x in _graph.Vertexes)
            //            {
            //                if (x.ConnectedEdges.Count > maxEdges)
            //                {
            //                    maxEdges = x.ConnectedEdges.Count;
            //                    maxEdgesVertex = x;
            //                }
            //            }

            //            _graph.Vertexes[i].ConnectedEdges.Add(new Edge
            //            {

            //                EndPoint = new Point(r.Next(0,400),r.Next(0,400)),
            //                StartPoint = new Point(r.Next(0, 400), r.Next(0, 400)),
            //                StartVertex = _graph.Vertexes[i],
            //                EndVertex = maxEdgesVertex,
            //                IdEdge = i
            //            });

            //            _graph.Vertexes[i].ConnectedVertexes.Add(maxEdgesVertex);
            //            degreeSequence[i]--;
            //            sum--;
            //        } 
            //    }
            //}

            //vertexIndex = _graph.Vertexes.Count;
            //foreach (var x in _graph.Vertexes)
            //{
            //    x.IdVertex = vertexIndex++;
            //}
            var window = obj as Window;
          //  DialogResult = true;
            window.Close();
        }

        public bool IsEven(object obj) // Parzysta
        {
            string sequenceString = (string)obj;
            List<int> degreeSequence = SplitDegreeSequence(sequenceString);
            int sum = degreeSequence.Sum();
            return sum%2 == 0;
        }

        public List<int> SplitDegreeSequence(string str)
        {
            return str.Split(',').Select(int.Parse).ToList();
        } 
    }
}
