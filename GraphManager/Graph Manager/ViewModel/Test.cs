using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Graph_Manager.Model;

namespace Graph_Manager.ViewModel
{
    public class Employee
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public Employee(string x, string y)
        {
            Imie = x;
            Nazwisko = y;
        }
    }

    public class Machine
    {
        public string Imie { get; set; }
        public string Marka { get; set; }
        public Machine(string x, string y)
        {
            Imie = x;
            Marka = y;
        }
    }

    public class Test
    {
        public ObservableCollection<Vertex> data1 { get; set; }
        public ObservableCollection<Edge> data2 { get; set; }
        public CompositeCollection coll { get; set; }
        public ICommand ImageMouseRightButtonDownCommand { get; set; }

        public Test()
        {
            ImageMouseRightButtonDownCommand = new RelayCommand(ImageMouseRightButtonDown, (n) => true);
            data1 = new ObservableCollection<Vertex>();
            
            data1.Add( new Vertex {  Margin = new Thickness(300, 50, 0, 0) });
            data1.Add(new Vertex {  Margin = new Thickness(300, 300, 0, 0) });

            data2 = new ObservableCollection<Edge>();
          
         
       //     data2.Add(new Edge { StartPointX = 50, EndPointX = 300, EndPointY = 50, StartPointY = 50 });
           
        //    data2.Add(new Edge { StartPointX = 0, EndPointX = 300, EndPointY = 300, StartPointY = 0 });


            coll = new CompositeCollection();
            coll.Add(new CollectionContainer() { Collection = data1 });
            coll.Add(new CollectionContainer() { Collection = data2 });
            // Data = coll;
        }

        private void ImageMouseRightButtonDown(object obj)
        {
            Canvas canvas = obj as Canvas;
            Point point = Mouse.GetPosition(canvas);
            point.X = point.X;// - Convert.ToDouble(Resources.ImageWidth) / 2;
            point.Y = point.Y; //- Convert.ToDouble(Resources.ImageHeight) / 2;
                MessageBox.Show(Convert.ToString(point));
        }
    }
}
