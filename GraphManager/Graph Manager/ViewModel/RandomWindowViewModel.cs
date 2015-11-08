using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    public class RandomWindowViewModel
    {
        public bool ReadTo { get; set; }
        private Graph _graph;
        private bool _onCircle;
        private int _canvasWidth;
        private int _canvasHeight;
        private Regex _expression;

        public RandomWindowViewModel(Graph graph, int canvasWidth, int canvasHeight)
        {
            ReadTo = false;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _graph = graph;
            _expression = new Regex("[1-9]+([,]{1}[1-9]+)*");
            OnCircle = true;
            RandomizeCommand = new RelayCommand(Randomize, Validation.IsEven);
            CloseCommand = new RelayCommand(o => ((Window) o).Close());

        }

        public bool OnCircle
        {
            get { return _onCircle; }
            set { _onCircle = value; }
        }

        public ICommand RandomizeCommand { get; set; }
        public ICommand CloseCommand;

        public void Randomize(object obj)
        {
            ReadTo = true;
            string sequenceString = (string) obj;
            List<int> degreeSequence = Validation.SplitSequence(sequenceString);
            int maxEdgesIndex = 0,
                maxDegree = 0,
                radius,
                vertexIndex,
                angleChange = 0,
                angle,
                sum = degreeSequence.Sum();
            Vertex maxEdgesVertex = new Vertex();
            Edge newEdge;
            Random r = new Random();
            Point p = new Point();
            angle = 360/degreeSequence.Count;

            for (int i = 0; i < degreeSequence.Count; i++)
            {
                if (OnCircle)
                {
                    radius = (_canvasHeight/2) - 20;
                    p.X = 0.5*_canvasWidth + (radius*Math.Sin(angleChange));
                    p.Y = 0.5*_canvasHeight + (radius*Math.Cos(angleChange));
                    angleChange += angle;
                }
                else
                {
                    p.X = r.Next(10, _canvasWidth - 10);
                    p.Y = r.Next(10, _canvasHeight - 10);
                }

                _graph.Vertexes.Add(new Vertex {Position = p});
            }

            while (sum > 0)
            {
                for (int k = 0; k < degreeSequence.Count; k++)
                {
                    maxDegree = 0;

                    if (_graph.Vertexes[k].ConnectedEdges.Count < degreeSequence[k])
                    {
                        for (int j = 0; j < _graph.Vertexes.Count; j++)
                        {

                            if (_graph.Vertexes[j].ConnectedEdges.Count >= maxDegree &&
                                _graph.Vertexes[j].ConnectedEdges.Count < degreeSequence[j] &&
                                j != k)
                            {
                                maxDegree = degreeSequence[j];
                                maxEdgesVertex = _graph.Vertexes[j];
                                maxEdgesIndex = j;
                            }
                        }

                        newEdge = new Edge
                        {
                            EdgeEnd = _graph.Vertexes[k].Position,
                            EdgeStart = maxEdgesVertex.Position,
                            StartVertex = _graph.Vertexes[k],
                            EndVertex = maxEdgesVertex,
                            IdEdge = k
                        };

                        _graph.Vertexes[k].ConnectedEdges.Add(newEdge);
                        maxEdgesVertex.ConnectedEdges.Add(newEdge);

                        _graph.Vertexes[k].ConnectedVertexes.Add(maxEdgesVertex);
                        maxEdgesVertex.ConnectedVertexes.Add(_graph.Vertexes[k]);

                        degreeSequence[k]--;
                        degreeSequence[maxEdgesIndex]--;
                        sum -= 2;
                    }
                }
            }

            vertexIndex = _graph.Vertexes.Count;
            foreach (var x in _graph.Vertexes)
            {
                x.IdVertex = vertexIndex++;
            }
            
        }
    }
}
