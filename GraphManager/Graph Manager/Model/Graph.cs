﻿using System;
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

        public List<Vertex> Vertexes { get; set; }
        public List<Edge> Edges { get; set; }
    }
}