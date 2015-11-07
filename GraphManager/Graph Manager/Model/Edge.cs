using System;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.ViewModel;

namespace Graph_Manager.Model
{
    public class Edge
    {
        public ICommand LineMouseLeftButtonDownCommand { get; set; }
        public int IdEdge { get; set; }
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        public bool IsMouseLeftButtonDown { get; set; }
        public ICommand OnEnterChangeCursorCommand { get; set; }
        public ICommand OnLeaveChangeCursorCommand { get; set; }
        public Point EdgeStart { get; set; }
        public Point EdgeEnd { get; set; }

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
        public void CalculateStartEndPoint()
        {
            double DiagonalBig = Math.Sqrt(Math.Pow(EndVertex.Position.X - StartVertex.Position.X, 2)
                + Math.Pow(EndVertex.Position.Y - StartVertex.Position.Y, 2));
            Point pointStart = new Point();
            Point pointEnd = new Point();
            pointStart.X = (EndVertex.Position.X - StartVertex.Position.X) / DiagonalBig * Convert.ToUInt32(Resources.Radius) + StartVertex.Position.X;
            pointStart.Y = (EndVertex.Position.Y - StartVertex.Position.Y) / DiagonalBig * Convert.ToUInt32(Resources.Radius) + StartVertex.Position.Y;
            pointEnd.X = EndVertex.Position.X - (EndVertex.Position.X - StartVertex.Position.X) / DiagonalBig * Convert.ToUInt32(Resources.Radius);
            pointEnd.Y = EndVertex.Position.Y - (EndVertex.Position.Y - StartVertex.Position.Y) / DiagonalBig * Convert.ToUInt32(Resources.Radius);
            pointStart.X = pointStart.X + Convert.ToDouble(Resources.ImageWidth) / 2;
            pointStart.Y = pointStart.Y + Convert.ToDouble(Resources.ImageHeight) / 2;
            pointEnd.X = pointEnd.X + Convert.ToDouble(Resources.ImageWidth) / 2;
            pointEnd.Y = pointEnd.Y + Convert.ToDouble(Resources.ImageHeight) / 2;

            EdgeStart = pointStart;
            EdgeEnd = pointEnd;
        }
    }
}
