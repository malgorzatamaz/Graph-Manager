using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    internal class PruferWindowViewModel
    {

        public bool ReadTo { get; set; }
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
            string sequenceString = (string) obj;
            List<int> pruferCodeList = Validation.SplitSequence(sequenceString);
            int radius, angleChange = 0, angle, index;
            Point p = new Point();
            Random r = new Random();
            angle = 360/pruferCodeList.Count;
            Edge newEdge;

            for (int i = 0; i < pruferCodeList.Count; i++)
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


            for (int i = 0; i < pruferCodeList.Count; i++)
            {
                index = pruferCodeList[i];

                newEdge = new Edge
                {
                    EndPoint = _graph.Vertexes[i].Position,
                    StartPoint = _graph.Vertexes[index].Position,
                    StartVertex = _graph.Vertexes[i],
                    EndVertex = _graph.Vertexes[index],
                    IdEdge = i
                };

                _graph.Vertexes[i].ConnectedEdges.Add(newEdge);
                _graph.Vertexes[index].ConnectedEdges.Add(newEdge);
            }
        }
    }
}
