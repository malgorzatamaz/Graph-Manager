using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace point_draw.Graphs
{
    class Graph
    {
        public List<Vertex> Vertexes;


        public Graph()
        {
            Vertexes = new List<Vertex>();
        }
        public int GetIndex(int canvasIndex)
        {
            return Vertexes.FindIndex(n => n.CanvasIndex == canvasIndex);
        }

        public int GetCanvasIndex(int index)
        {
            return Vertexes[index].CanvasIndex;
        }
    }

    
}
