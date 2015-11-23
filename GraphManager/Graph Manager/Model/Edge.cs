using System;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graph_Manager.Model
{
    public class Edge
    {
        public int EdgeId { get; set; }
        public int IdEdge { get; set; }
        public int StartVertexId { get; set; }
        public int EndVertexId { get; set; }
        public bool IsMouseLeftButtonDown { get; set; }
        public ICommand LineMouseLeftButtonDownCommand { get; set; }
        public ICommand OnEnterChangeCursorCommand { get; set; }
        public ICommand OnLeaveChangeCursorCommand { get; set; }
        public Point StartPoint { get; set; }
        public string StartPoint_String { get; set; }
        public Point EndPoint { get; set; }
        public string EndPoint_String { get; set; }

        public virtual Graph Graph { get; set; }
        public int GraphId { get; set; }

        public Edge()
        {
            LineMouseLeftButtonDownCommand = new RelayCommand((m)=> IsMouseLeftButtonDown = true, (n) => true);
            OnEnterChangeCursorCommand = new RelayCommand(OnEnterChangeCursor, (n) => true);
            OnLeaveChangeCursorCommand = new RelayCommand(OnLeaveChangeCursor, (n) => true);
        }

        private void OnLeaveChangeCursor(object obj)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void OnEnterChangeCursor(object obj)
        {
            if (MainWindowViewModel.IndexAction == 3)
                Mouse.OverrideCursor = Cursors.Cross;
        }

        /// <summary>
        /// sluzy do obcinania line, zeby linii nie bylo na obrazku + przemieszczenie do centru obrazka
        /// </summary>
        /// <param name="edge"></param>
        public void CalculateStartEndPoint(Graph graph)
        {
            Vertex startVertex = new Vertex();
            Vertex endVertex = new Vertex();
            foreach (var v in graph.Vertexes)
            {
                if (v.IdVertex == StartVertexId)
                    startVertex = v;
                else if(v.IdVertex == EndVertexId)
                    endVertex = v;
            }


            double DiagonalBig = Math.Sqrt(Math.Pow(endVertex.Position.X - startVertex.Position.X, 2)
                + Math.Pow(endVertex.Position.Y - startVertex.Position.Y, 2));
            Point pointStart = new Point();
            Point pointEnd = new Point();
            pointStart.X = (endVertex.Position.X - startVertex.Position.X) / DiagonalBig * Convert.ToUInt32(Resources.Radius) + startVertex.Position.X;
            pointStart.Y = (endVertex.Position.Y - startVertex.Position.Y) / DiagonalBig * Convert.ToUInt32(Resources.Radius) + startVertex.Position.Y;
            pointEnd.X = endVertex.Position.X - (endVertex.Position.X - startVertex.Position.X) / DiagonalBig * Convert.ToUInt32(Resources.Radius);
            pointEnd.Y = endVertex.Position.Y - (endVertex.Position.Y - startVertex.Position.Y) / DiagonalBig * Convert.ToUInt32(Resources.Radius);
            pointStart.X = pointStart.X + Convert.ToDouble(Resources.ImageWidth) / 2;
            pointStart.Y = pointStart.Y + Convert.ToDouble(Resources.ImageHeight) / 2;
            pointEnd.X = pointEnd.X + Convert.ToDouble(Resources.ImageWidth) / 2;
            pointEnd.Y = pointEnd.Y + Convert.ToDouble(Resources.ImageHeight) / 2;

            StartPoint = pointStart;
            EndPoint = pointEnd;
        }
    }
}
