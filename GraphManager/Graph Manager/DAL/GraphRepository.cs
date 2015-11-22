using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph_Manager.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.RegularExpressions;

namespace Graph_Manager.DAL
{
    public class GraphRepository : IGraphRepository
    {
        public static int _savedGraph { get; set; }

        private GraphContext _graphContext = new GraphContext();

        private int _graphId { get; set; }
        public int VertexesNumber { get; set; }
        public int EdgesNumber { get; set; }

        public Graph Graph { get; set; }
        public List<Vertex> Vertexes { get; set; }
        public List<Edge> Edges { get; set; }

        private ThicknessConverter _thicknessConverter { get; set; }
        public void SaveGraph(Graph graph)
        {

            _graphContext.Graphs.Add(new Graph
            {
                GraphName = graph.GraphName,
            });
            int graphId = _graphContext.Graphs.Where(n => n.GraphName == graph.GraphName).
                Select(n => n.GraphId).FirstOrDefault();

            foreach (var vertex in graph.Vertexes)
            {
                string positionX = vertex.Position.X.ToString().Split(',')[0].Trim();
                string positionY = vertex.Position.Y.ToString().Split(',')[0].Trim();
                string position = positionX + ',' + positionY;
                string marginT = vertex.Margin.Top.ToString().Split(',')[0].Trim();
                string marginB = vertex.Margin.Bottom.ToString().Split(',')[0].Trim();
                string marginL = vertex.Margin.Left.ToString().Split(',')[0].Trim();
                string marginR = vertex.Margin.Right.ToString().Split(',')[0].Trim();

                string margin = marginL + ';' + marginT + ';' + marginR + ';' + marginB;

                _graphContext.Vertexes.Add(new Vertex
                {
                    Position_string = position,
                    Margin_string = margin,
                    IdVertex = vertex.IdVertex,
                    GraphId = graphId
                });
            }

            foreach (var edge in graph.Edges)
            {
                string startPointX = edge.StartPoint.X.ToString().Split(',')[0].Trim();
                string startPointY = edge.StartPoint.Y.ToString().Split(',')[0].Trim();
                string endpointX = edge.EndPoint.X.ToString().Split(',')[0].Trim();
                string endpointY = edge.EndPoint.Y.ToString().Split(',')[0].Trim();

                string startPoint = startPointX + ',' + startPointY;
                string endPoint = endpointX + ',' + endpointY;

                _graphContext.Edges.Add(new Edge
                {
                    IdEdge = edge.IdEdge,
                    StartPoint_String = startPoint,
                    EndPoint_String = endPoint,
                    StartVertexId = edge.StartVertexId,
                    EndVertexId = edge.EndVertexId,
                    GraphId = graphId
                });
            }
            _graphContext.SaveChanges();
        }

        public bool CheckFileNames(string fileName)
        {
            var name = _graphContext.Graphs.FirstOrDefault(g => g.GraphName == fileName);
            if (name != null)
                return false;
            else
                return true;
        }

        public void GetFileNames(ObservableCollection<string> fileNameList)
        {
            var fileNames = _graphContext.Graphs.Select(n => n.GraphName).ToList();
            foreach (var name in fileNames)
                fileNameList.Add(name);
        }

        public void LoadGraph(Graph graph)
        {
            Vertexes = _graphContext.Vertexes.Where(n => n.GraphId == _graphId).ToList();
            Edges = _graphContext.Edges.Where(n => n.GraphId == _graphId).ToList();

            var startVertexList = Edges.GroupBy(n => n.StartVertexId).Select(n => n.FirstOrDefault()).ToList();
            var endVertexList = Edges.GroupBy(n => n.EndVertexId).Select(n => n.FirstOrDefault()).ToList();
            Dictionary<int, List<int>> connectedVertexesIdDict = new Dictionary<int, List<int>>();
            //dodawanie wierzcholkow polaczonych ze soba do slownika
            foreach (var vertex in startVertexList)
            {
                connectedVertexesIdDict.Add(
                    vertex.StartVertexId,
                    new List<int>(Edges.Where(n => n.StartVertexId == vertex.StartVertexId).Select(n => n.EndVertexId).ToList()));
            }
            foreach (var vertex in endVertexList)
            {
                var ends = Edges.Where(n => n.EndVertexId == vertex.EndVertexId).Select(n => n.StartVertexId).ToList();
                if (connectedVertexesIdDict.ContainsKey(vertex.EndVertexId))
                {
                    foreach (var end in ends)
                        connectedVertexesIdDict[vertex.EndVertexId].Add(end);
                }
                else
                    connectedVertexesIdDict.Add(
                        vertex.EndVertexId,
                        new List<int>(ends));
            }

            //dodawanie krawedzi do poszczegolnych wierzcholkow
            Dictionary<int, List<int>> connectedEdgesIdDict = new Dictionary<int, List<int>>();
            foreach (var vertex in startVertexList)
            {
                connectedEdgesIdDict.Add(
                    vertex.StartVertexId,
                    new List<int>(Edges.Where(n => n.StartVertexId == vertex.StartVertexId).Select(n => n.IdEdge).ToList()));
            }
            foreach (var vertex in endVertexList)
            {
                var ends = Edges.Where(n => n.EndVertexId == vertex.EndVertexId).Select(n => n.IdEdge).ToList();
                if (connectedEdgesIdDict.ContainsKey(vertex.EndVertexId))
                {
                    foreach (var end in ends)
                        connectedEdgesIdDict[vertex.EndVertexId].Add(end);
                }
                else
                    connectedEdgesIdDict.Add(
                        vertex.EndVertexId,
                        new List<int>(ends));
            }

            _thicknessConverter = new ThicknessConverter();


            foreach (var vertex in Vertexes)
            {
                //znajdywanie wierzcholkow polaczonych z aktualnie wskazanym
                List<Vertex> connectedVertexes = new List<Vertex>();
                if (connectedVertexesIdDict.ContainsKey(vertex.IdVertex))
                {
                    foreach (var v in connectedVertexesIdDict[vertex.IdVertex])
                        connectedVertexes.Add(Vertexes.FirstOrDefault(n => n.IdVertex == v));
                }

                //znajdywanie krawedzi polaczonych z aktualnie wskazanym wierzcholkiem
                List<Edge> connectedEdges = new List<Edge>();
                if (connectedEdgesIdDict.ContainsKey(vertex.IdVertex))
                {
                    foreach (var e in connectedEdgesIdDict[vertex.IdVertex])
                        connectedEdges.Add(Edges.FirstOrDefault(n => n.IdEdge == e));
                }

                graph.Vertexes.Add(new Vertex
                {
                    Margin = (Thickness)_thicknessConverter.ConvertFromString(vertex.Margin_string),
                    Position = Point.Parse(vertex.Position_string),
                    IdVertex = vertex.IdVertex,
                    ConnectedVertexes = connectedVertexes,
                    ConnectedEdges = connectedEdges,
                    
                });
            }

            foreach (var edge in Edges)
            {
                graph.Edges.Add(new Edge
                {
                    IdEdge = edge.IdEdge,
                    StartPoint = Point.Parse(edge.StartPoint_String),
                    EndPoint = Point.Parse(edge.EndPoint_String),
                    StartVertexId = edge.StartVertexId,
                    EndVertexId = edge.EndVertexId,
                });
            }

        }

        public void ShowGraphDetails(string fileName)
        {
            VertexesNumber = 0;
            EdgesNumber = 0;

            Graph = new Graph();
            Graph = _graphContext.Graphs.FirstOrDefault(n => n.GraphName == fileName);
            _graphId = Graph.GraphId;
            VertexesNumber = _graphContext.Vertexes.Where(n => n.GraphId == _graphId).Count();
            EdgesNumber = _graphContext.Edges.Where(n => n.GraphId == _graphId).Count();



        }
    }
}
