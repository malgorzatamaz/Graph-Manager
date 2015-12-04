using Graph_Manager.DAL;
using Graph_Manager.Model;
using Graph_Manager.View;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Graph_Manager.ViewModel
{
    [ImplementPropertyChanged]
    public class LoadGraphViewModel
    {
        private int _selectedIndexFileNames;
        private bool _isRunning { get; set; }
        private Graph _graph { get; set; }
        private GraphRepository _graphRepository { get; set; }
        public LoadGraphWindow Window { get; set; }
        public bool ReadTo { get; set; }

        public string VertexesNumber { get; set; }
        public string EdgesNumber { get; set; }
        public string GraphSequence { get; set; }
        public string GraphConnectivity { get; set; }

        public ObservableCollection<string> FileNameList { get; set; }

        public ICommand LoadGraphCommand { get; set; }
        public ICommand DeleteGraphCommand { get; set; }

        public int SelectedIndexFileNames
        {
            get { return _selectedIndexFileNames; }
            set
            {
                _selectedIndexFileNames = value;
                ShowGraphProperties();
            }
        }
        public LoadGraphViewModel(Graph graph)
        {
            LoadGraphCommand = new RelayCommand(LoadGraph, (n) => true);
            DeleteGraphCommand = new RelayCommand(DeleteGraph, (n) => true);
            _graph = graph;
            _graphRepository = new GraphRepository();
            FileNameList = new ObservableCollection<string>();
            _graphRepository.GetFileNames(FileNameList);
            SelectedIndexFileNames = 0;
            ShowGraphProperties();
        }

        private void DeleteGraph(object obj)
        {
            if (FileNameList.Any())
            {
                DialogResult dialog = MessageBox.Show("Czy chcesz napewno usunąć plik '" + FileNameList[SelectedIndexFileNames] + "'?", 
                    "Usunięcie grafu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    _graphRepository.DeleteGraph();
                    FileNameList = new ObservableCollection<string>();
                    _graphRepository.GetFileNames(FileNameList);
                    SelectedIndexFileNames = 0;
                    ShowGraphProperties();  
                }
            }

        }

        private void LoadGraph(object obj)
        {
            ReadTo = true;
            _graphRepository.LoadGraph(_graph);
            Window.Close();
        }

        private void ShowGraphProperties()
        {
            if (FileNameList.Any())
            {
                _graphRepository.ShowGraphDetails(FileNameList[SelectedIndexFileNames]);
                VertexesNumber = _graphRepository.VertexesNumber.ToString();
                EdgesNumber = _graphRepository.EdgesNumber.ToString();
                GraphSequence = _graphRepository.GraphSequence;
                GraphConnectivity = _graphRepository.GraphConnectivity;
            }
            else
            {
                VertexesNumber = string.Empty;
                EdgesNumber = string.Empty;
                GraphSequence = string.Empty;
                GraphConnectivity = string.Empty;
            }

        }

    }
}
