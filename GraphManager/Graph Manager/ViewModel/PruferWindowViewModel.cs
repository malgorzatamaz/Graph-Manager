using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    class PruferWindowViewModel
    {

        public bool ReadTo { get; set; }
        private Graph _graph;
        private bool _onCircle;
        private int _canvasWidth;
        private int _canvasHeight;
        private Regex _expression;


        public PruferWindowViewModel(Graph graph, int canvasWidth, int canvasHeight)
        {
            ReadTo = false;
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _graph = graph;
            _expression = new Regex("[1-9]+([,]{1}[1-9]+)*");
            OnCircle = true;
            RandomizeGraph = new RelayCommand(Recreation, IsEven);
            CloseCommand = new RelayCommand(o => ((Window)o).Close());

        }

        public bool OnCircle
        {
            get { return _onCircle; }
            set { _onCircle = value; }
        }
        public ICommand RandomizeGraph { get; set; }
        public ICommand CloseCommand;

        public void Recreation(object obj)
        {
            
        }
        public bool IsEven(object obj) // Parzysta
        {
            string sequenceString = (string)obj;
            if (!string.IsNullOrEmpty(sequenceString))
            {
                if (sequenceString.Last() != ',' && _expression.IsMatch(sequenceString))
                {
                    return true;
                }
            }

            return false;
        }

        public List<int> SplitDegreeSequence(string str)
        {

            return str.Split(',').Select(int.Parse).ToList();
        }
    }
}
