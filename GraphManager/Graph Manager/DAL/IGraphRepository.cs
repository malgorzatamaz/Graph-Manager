using Graph_Manager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Manager.DAL
{
    interface IGraphRepository
    {
        void SaveGraph(Graph graph, string degreeSequence);
        void LoadGraph(Graph graph);
        void ShowGraphDetails(string fileName);
        void GetFileNames(ObservableCollection<string> fileNameList);
        bool CheckFileNames(string fileName);
        void DeleteGraph();
    }
}
