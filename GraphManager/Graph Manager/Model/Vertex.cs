using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.ViewModel;
using PropertyChanged;

namespace Graph_Manager.Model
{
    [ImplementPropertyChanged]
    public class Vertex
    {
        public ICommand ImageMouseRightButtonDownCommand { get; set; }
        public ICommand ImageMouseLeftButtonDownCommand { get; set; }
        private Func<int> CallCanvasMouseRightButtonDown;
        public Vertex()
        {
            ConnectedEdges = new List<Edge>();
            ConnectedVertexes = new List<Vertex>();
            Selected = false;
            Path = AppDomain.CurrentDomain.BaseDirectory + "CircleBlue.png";
            ImageMouseRightButtonDownCommand = new RelayCommand(ImageMouseRightButtonDown, (n) => true);
            ImageMouseLeftButtonDownCommand = new RelayCommand(ImageMouseLeftButtonDown, (n) => true);
        }

        private void ImageMouseRightButtonDown(object obj)
        {
            IsMouseRightButtonDown = true;
            if (Selected == false)
            {
                Selected = true;
                Path = AppDomain.CurrentDomain.BaseDirectory + "CircleOrange.png"; ;
            }
            else
            {
                Selected = false;
                Path = AppDomain.CurrentDomain.BaseDirectory + "CircleBlue.png";
            }
        }

        private void ImageMouseLeftButtonDown(object obj)
        {
            IsMouseRightButtonDown = true;
        }

        public bool IsMouseRightButtonDown { get; set; }
        public bool IsMouseLeftButtonDown { get; set; }
        public int IdVertex { get; set; }
        public Thickness Margin { get;  set; }
        public Point Position { get; set; }
        public string Path { get; set; }
        public List<Vertex> ConnectedVertexes { get; set; }
        public List<Edge> ConnectedEdges { get; set; }
        public Boolean Selected { get; set; } //przechowuje informacje czy wierzchołek jest zaznaczony 
    }
}
