using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Manager.Model
{
    public class GraphContext : DbContext
    {
        public DbSet<Graph> Graphs { get; set; }
        public DbSet<Vertex> Vertexes { get; set; }
        public DbSet<Edge> Edges { get; set; }

        public GraphContext()
        {
            Database.SetInitializer<GraphContext>(new DropCreateDatabaseIfModelChanges<GraphContext>());
        }
    }
}
