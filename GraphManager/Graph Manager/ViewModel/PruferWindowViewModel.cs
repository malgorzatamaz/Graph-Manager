using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;
using Graph_Manager.View;

namespace Graph_Manager.ViewModel
{
    public class PruferWindowViewModel
    {

        public bool ReadTo { get; set; }
        public PruferWindow Window { get; set; }
        private Graph _graph;
        private bool _onCircle;
        private int _canvasWidth;
        private int _canvasHeight;
        private Regex _expression;


        public PruferWindowViewModel(Graph graph, int canvasWidth, int canvasHeight)
        {
            ReadTo = false;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _graph = graph;
            _expression = new Regex("[1-9]+([,]{1}[1-9]+)*");
            OnCircle = true;
            RecreateCommand = new RelayCommand(Recreation, Validation.IsPruferCode);
            CloseCommand = new RelayCommand(o => ((Window) o).Close());
        }

        public bool OnCircle
        {
            get { return _onCircle; }
            set { _onCircle = value; }
        }

        public ICommand RecreateCommand { get; set; }
        public ICommand CloseCommand;

        public void Recreation(object obj)
        {
            ReadTo = true;
            string sequenceString = (string) obj;
            List<int> pruferCodeList = Validation.SplitSequence(sequenceString);
            List<int> Vertexes = new List<int>();
            int min,radius, angleChange = 0, angle, index,c;
            Point p = new Point();
            Random r = new Random();

            angle = 360/pruferCodeList.Count;
            Edge newEdge;

            for (int i = 0; i < pruferCodeList.Count ; i++)
            {
                Vertexes[i] = i;
            }
            
            for (int i = 0; i < pruferCodeList.Count; i++)
            {
                if (OnCircle)
                {
                    radius = (_canvasHeight) - 20;
                    p.X = 0.5*_canvasWidth + (radius*Math.Sin(angleChange));
                    p.Y = 0.5*_canvasHeight + (radius*Math.Cos(angleChange));
                    angleChange += angle;
                }
                else
                {
                    p.X = r.Next(10, _canvasWidth - 10);
                    p.Y = r.Next(10, _canvasHeight - 10);
                }

                _graph.Vertexes.Add(new Vertex {Position = p, Margin = new Thickness(p.X,p.Y,0,0), IdVertex = i });
            }

   
            c = 0;

            for (int i = 0; i < pruferCodeList.Count; i++)
            {
                min = 100000;
                index = 0;

                for (int j = 0; j < Vertexes.Count; j++)
                {
                    if (Vertexes[i] < min && !pruferCodeList.Contains(Vertexes[i]))
                    {
                        min = Vertexes[i];
                        index = i;
                    } 
                }

                if (c != 0)
                {
                    pruferCodeList.RemoveAt(c);
                    Vertexes.RemoveAt(index);

                    newEdge = new Edge
                    {
                        StartVertexId = _graph.Vertexes[c].IdVertex,
                        EndVertexId = _graph.Vertexes[index].IdVertex,
                        IdEdge = index
                    };

                    newEdge.CalculateStartEndPoint(_graph);
                    _graph.Edges.Add(newEdge);

                    _graph.Vertexes[index].ConnectedEdges.Add(newEdge);
                    _graph.Vertexes[c].ConnectedEdges.Add(newEdge);
                    _graph.Vertexes[index].ConnectedVertexes.Add(_graph.Vertexes[c]);
                    _graph.Vertexes[c].ConnectedVertexes.Add(_graph.Vertexes[index]);

                    c++;
                }
                else
                {
                    newEdge = new Edge
                    {
                        StartVertexId = _graph.Vertexes[Vertexes[0]].IdVertex,
                        EndVertexId = _graph.Vertexes[Vertexes[1]].IdVertex,
                        IdEdge = index
                    };

                    newEdge.CalculateStartEndPoint(_graph);
                    _graph.Edges.Add(newEdge);

                    _graph.Vertexes[Vertexes[0]].ConnectedEdges.Add(newEdge);
                    _graph.Vertexes[Vertexes[1]].ConnectedEdges.Add(newEdge);
                    _graph.Vertexes[Vertexes[0]].ConnectedVertexes.Add(_graph.Vertexes[c]);
                    _graph.Vertexes[Vertexes[1]].ConnectedVertexes.Add(_graph.Vertexes[index]);
                }

            }

            Window.Close();
        }
    }
}
