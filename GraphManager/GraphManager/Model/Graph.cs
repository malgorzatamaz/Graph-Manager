using System.Collections.Generic;

namespace GraphManager.Model
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
