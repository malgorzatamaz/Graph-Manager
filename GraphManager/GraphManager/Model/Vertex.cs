using System.Collections.Generic;

namespace GraphManager.Model
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
