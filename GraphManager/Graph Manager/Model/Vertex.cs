using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Graph_Manager.Model
{
    public class Vertex
    {
        public int Index { get; set; }
        public Point Position { get; set; }
        public List<Vertex> ConnectedVertexes { get; set; }
        public List<Edge> ConnectedEdges { get; set; }
    }
}
