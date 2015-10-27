using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    class RandomMainViewModel
    {
        private Graph _graph;
        private int[] _tab; 

        public RandomMainViewModel(Graph graph, int [] tab)
        {
            _graph = graph;
            _tab = tab;
            RandomizeGraph = new RelayCommand(Randomize, (m)=>true);
        }


        public ICommand RandomizeGraph { get; set; }

        public void Randomize(object obj)
        {
            
        }

        public bool IsEven() // Parzysta
        {
            int sum = 0;
            for (int i = 0; i < _tab.Length; i++)
            {
                sum += _tab[i];
            }
            return sum%2 == 0;
        }


    }
}
