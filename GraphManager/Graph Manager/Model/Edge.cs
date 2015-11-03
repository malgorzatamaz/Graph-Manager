using System;
using System.Windows;

namespace Graph_Manager.Model
{
    public class Edge
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int IdEdge { get; set; }
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
    }
}
