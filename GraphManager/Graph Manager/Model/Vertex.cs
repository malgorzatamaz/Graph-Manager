using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.ViewModel;
using PropertyChanged;
using System.IO;
using System.Windows.Controls;

namespace Graph_Manager.Model
{
    [ImplementPropertyChanged]
    public class Vertex
    {
        public ICommand ImageMouseRightButtonDownCommand { get; set; }
        public ICommand ImageMouseLeftButtonDownCommand { get; set; }
        public ICommand OnEnterChangeCursorCommand { get; set; }
        public ICommand OnLeaveChangeCursorCommand { get; set; }
        public Vertex()
        {
            ConnectedEdges = new List<Edge>();
            ConnectedVertexes = new List<Vertex>();
            Selected = false;
            PathDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            Path = PathDirectory + Resources.CircleBlue;
            ImageMouseRightButtonDownCommand = new RelayCommand(ImageMouseRightButtonDown, (n) => true);
            ImageMouseLeftButtonDownCommand = new RelayCommand((m) => IsMouseLeftButtonDown = true, (n) => true);
            OnEnterChangeCursorCommand = new RelayCommand(OnEnterChangeCursor, (n) => true);
            OnLeaveChangeCursorCommand = new RelayCommand(OnLeaveChangeCursor, (n) => true);
        }

        private void ImageMouseRightButtonDown(object obj)
        {
            IsMouseRightButtonDown = true;
            if (Selected == false)
            {
                Selected = true;
                Path = PathDirectory + Resources.CircleOrange;
            }
            else
            {
                Selected = false;
                Path = PathDirectory + Resources.CircleBlue;
            }
        }

        private void OnEnterChangeCursor(object obj)
        {
            var tool = new ToolTip();
            if (MainWindowViewModel.IndexAction == 2)
                Mouse.OverrideCursor = Cursors.Hand;
            else if (MainWindowViewModel.IndexAction == 3)
                Mouse.OverrideCursor = Cursors.Cross;
        }

        private void OnLeaveChangeCursor(object obj)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public string PathDirectory { get; set; }
        public bool IsMouseRightButtonDown { get; set; }
        public bool IsMouseLeftButtonDown { get; set; }
        public int IdVertex { get; set; }
        public Thickness Margin { get; set; }
        public Point Position { get; set; }
        public string Path { get; set; }
        public List<Vertex> ConnectedVertexes { get; set; }
        public List<Edge> ConnectedEdges { get; set; }
        public Boolean Selected { get; set; }

    }
}
