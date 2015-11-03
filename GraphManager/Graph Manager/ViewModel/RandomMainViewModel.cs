using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    class RandomMainViewModel
    {
        private Graph _graph;
    //  private int[] _tab; 
        
        public RandomMainViewModel(Graph graph)
        {
            _graph = graph;
        //    _tab = tab;
            RandomizeGraph = new RelayCommand(Randomize, IsEven);
        }


        public ICommand RandomizeGraph { get; set; }

        public void Randomize(object obj)
        {
            List<int> degreeSequence = (List<int>)obj;
            int maxEdges = 0, vertexIndex, sum = degreeSequence.Sum();
            Vertex maxEdgesVertex = new Vertex();
            Random r = new Random();


            foreach (var x in degreeSequence)
            {
                _graph.Vertexes.Add(new Vertex());
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
                            //EndPoint = new Point(r.Next(0,400),r.Next(0,400)),
                           // StartPoint = new Point(r.Next(0, 400), r.Next(0, 400)),
                            StartVertex = _graph.Vertexes[i],
                            EndVertex = maxEdgesVertex,
                            IdEdge = i
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
                x.IdVertex = vertexIndex++;
            }
        }

        public bool IsEven(object obj) // Parzysta
        {
            List<int> degreeSequence = (List<int>) obj;
            int sum = degreeSequence.Sum();
            return sum%2 == 0;
        }
    }
}
