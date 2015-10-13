using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace point_draw.Graphs
{
    class Vertex
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }
        public int CanvasIndex { get; set; }
        public List<Vertex> Neibourghs { get; set; }
        public List<int> EdgesIndex { get; set; } 

        public Vertex()
        {
             Neibourghs = new List<Vertex>();
            EdgesIndex = new List<int>();
        }

        
    }



}
