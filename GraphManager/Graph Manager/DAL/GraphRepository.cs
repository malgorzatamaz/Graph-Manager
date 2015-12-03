using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph_Manager.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.RegularExpressions;
using Graph_Manager.ViewModel;

namespace Graph_Manager.DAL
{
    public class GraphRepository : IGraphRepository
    {
        public static int _savedGraph { get; set; }

        private GraphContext _graphContext = new GraphContext();

        private int _graphId { get; set; }
        public int VertexesNumber { get; set; }
        public int EdgesNumber { get; set; }
        public string GraphSequence { get; set; }
        public string GraphConnectivity { get; set; }
        public int IdIamge { get; set; }
        public Graph Graph { get; set; }
        public List<Vertex> Vertexes { get; set; }
        public List<Edge> Edges { get; set; }

        private ThicknessConverter _thicknessConverter { get; set; }
        private Dictionary<int, List<int>> _connectedVertexesIdDict { get; set; }
        public void SaveGraph(Graph graph, string degreeSequence)
        {
            _graphContext.Graphs.Add(new Graph
            {
                GraphName = graph.GraphName,
                GraphSequence = degreeSequence
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
            MainWindowViewModel.IdImage = 0;
            MainWindowViewModel.IdEdge = 0;
             _thicknessConverter = new ThicknessConverter();

            Vertexes = _graphContext.Vertexes.Where(n => n.GraphId == _graphId).ToList();
            Edges = _graphContext.Edges.Where(n => n.GraphId == _graphId).ToList();

            var startVertexList = Edges.GroupBy(n => n.StartVertexId).Select(n => n.FirstOrDefault()).ToList();
            _connectedVertexesIdDict = new Dictionary<int, List<int>>();
            
            //dodawanie wierzcholkow polaczonych ze soba do slownika
            foreach (var vertex in startVertexList)
            {
                _connectedVertexesIdDict.Add(
                    vertex.StartVertexId,
                    new List<int>(Edges.Where(n => n.StartVertexId == vertex.StartVertexId).Select(n => n.EndVertexId).ToList()));
            }

            foreach (var vertex in Vertexes)
            {
                graph.Vertexes.Add(new Vertex
                {
                    Margin = (Thickness)_thicknessConverter.ConvertFromString(vertex.Margin_string),
                    Position = Point.Parse(vertex.Position_string),
                    IdVertex = vertex.IdVertex,
                
                });
                MainWindowViewModel.IdImage++;
            }

            foreach (var selected in _connectedVertexesIdDict.Keys)
            {
                var selectedVertex = graph.Vertexes.FirstOrDefault(n => n.IdVertex == selected);
                foreach (var conVer in _connectedVertexesIdDict[selected])
                {
                    var connectedVertex = graph.Vertexes.FirstOrDefault(n => n.IdVertex == conVer);
                    var edge = new Edge()
                    {
                        StartVertexId = selected,
                        EndVertexId = conVer,
                        IdEdge = MainWindowViewModel.IdEdge
                    };
                    edge.CalculateStartEndPoint(graph);
                    graph.Edges.Add(edge);
                    MainWindowViewModel.IdEdge++;
                    selectedVertex.ConnectedEdges.Add(edge);
                    selectedVertex.ConnectedVertexes.Add(connectedVertex);

                    connectedVertex.ConnectedEdges.Add(edge);
                    connectedVertex.ConnectedVertexes.Add(selectedVertex);
                    MainWindowViewModel.IdEdge++;  
                }
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

            if (Graph.GraphSequence.Contains("0"))
                GraphConnectivity = "Graf niespójny";
            else
                GraphConnectivity = "Graf spójny";

            GraphSequence = Graph.GraphSequence;




        }

        public void DeleteGraph()
        {
            _graphContext.Edges.RemoveRange(_graphContext.Edges.Where(e => e.GraphId == _graphId));
            _graphContext.Vertexes.RemoveRange(_graphContext.Vertexes.Where(v => v.GraphId == _graphId));
            _graphContext.Graphs.RemoveRange(_graphContext.Graphs.Where(g => g.GraphId == _graphId));
            _graphContext.SaveChanges();
        }
    }
}
