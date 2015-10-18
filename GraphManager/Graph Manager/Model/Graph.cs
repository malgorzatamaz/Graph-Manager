using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Manager.Model
{
    public class Graph
    {
        public Graph()
        {
            Vertexes = new List<Vertex>();
            Edges = new List<Edge>();
        }

        private List<Vertex> Vertexes { get; set; }
        private List<Edge> Edges { get; set; }
    }
}
