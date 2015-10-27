using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    class RandomViewModel
    {
        private Graph _graph;

        public RandomViewModel(Graph graph)
        {
            _graph = graph;
            RandomizeGraph = new RelayCommand(Randomize, IsEven);
        }

        public bool OnCircle{ get; set; }
        public int CircleCenterX { get; set; }
        public int CircleCenterY { get; set; }
        public int CanvasHeight { get; set; }
        public int CanvasWidth { get; set; }
        public ICommand RandomizeGraph { get; set; }

        public void Randomize(object obj)
        {
            string sequenceString = (string)obj;
            List<int> degreeSequence = SplitDegreeSequence(sequenceString);
            int maxEdges = 0, radius, vertexIndex, angleChange = 0, angle, sum = degreeSequence.Sum();
            Vertex maxEdgesVertex = new Vertex();
            Random r = new Random();
            Point p = new Point();
            angle = 360 / _graph.Vertexes.Count;

            foreach (var x in degreeSequence)
            {

                if (OnCircle)
                {
                    radius = CanvasHeight - 40;
                    p.X = 0.5 * CanvasWidth + (radius * Math.Sin(angleChange));
                    p.Y = 0.5 * CanvasHeight + (radius * Math.Cos(angleChange));
                    angleChange += angle;
                }
                else
                {
                    p.X = r.Next(10, CanvasWidth - 10);
                    p.Y = r.Next(10, CanvasHeight - 10);
                }

                _graph.Vertexes.Add(new Vertex {Position = p});
            }
            
            while (sum > -1)
            {
                for (int i = 0; i < _graph.Vertexes[i].ConnectedEdges.Count; i++)
                {
                    if (_graph.Vertexes[i].ConnectedEdges.Count < degreeSequence[i])
                    {
                        foreach (var x in _graph.Vertexes)
                        {
                            if (x.ConnectedEdges.Count > maxEdges)
                            {
                                maxEdges = x.ConnectedEdges.Count;
                                maxEdgesVertex = x;
                            }
                        }

                        _graph.Vertexes[i].ConnectedEdges.Add(new Edge
                        {
                            StartPoint = _graph.Vertexes[i].Position,
                            EndPoint = maxEdgesVertex.Position,
                            StartVertex = _graph.Vertexes[i],
                            EndVertex = maxEdgesVertex,
                            Index = i
                        });

                        _graph.Vertexes[i].ConnectedVertexes.Add(maxEdgesVertex);
                        degreeSequence[i]--;
                    } 
                }
                sum--;
            }

            vertexIndex = _graph.Vertexes.Count;
            foreach (var x in _graph.Vertexes)
            {
                x.Index = vertexIndex++;
            }
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
