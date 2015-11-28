using Graph_Manager.DAL;
using Graph_Manager.Model;
using Graph_Manager.View;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Graph_Manager.ViewModel
{
    [ImplementPropertyChanged]
    public class SaveToDatabaseViewModel
    {
        private Graph _graph { get; set; }
        public ICommand SaveCommand { get; set; }
        private GraphRepository _graphRepository { get; set; }
        public SaveToDatabaseWindow Window { get; set; }
        public bool IsName { get; set; }
        
        public SaveToDatabaseViewModel(Graph graph)
        {
            _graph = new Graph();
            _graph = graph;
            _graphRepository = new GraphRepository();
            SaveCommand = new RelayCommand(SaveToDatabase, CheckFileName);
        }

        private bool CheckFileName(object obj)
        {
            string fileName = (string)obj;
            IsName = _graphRepository.CheckFileNames(fileName);
            if (!string.IsNullOrEmpty(fileName) && IsName)
            {
                _graph.GraphName = fileName.ToString();
                return true;
            }

            return false;
        }

        private void SaveToDatabase(object obj)
        {
            GraphRepository graphrepository = new GraphRepository();
            graphrepository.SaveGraph(_graph);
            Window.Close();
        }
    }
}
