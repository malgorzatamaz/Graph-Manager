using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Graph_Manager.Model
{
    public class Edge
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int Index { get; set; }
        public Vertex StartVertex { get; set; } //informacja nadmmiarowa
        public Vertex EndVertex { get; set; } //informacja nadmiarowa 
    }
}
