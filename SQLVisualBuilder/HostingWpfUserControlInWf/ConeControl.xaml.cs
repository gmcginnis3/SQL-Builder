using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace HostingWpfUserControlInWf
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        Table leftTable { get; set; }
        Table rightTable { get; set; }

        public UserControl1(string left, List<string> cols)
        {
            leftTable = new Table();
            leftTable.header = left;

            Binding MyBinding = new Binding();
            MyBinding.Path = new PropertyPath("header");
            MyBinding.Source = leftTable;
            groupBox1.SetBinding(GroupBox.HeaderProperty, MyBinding);
            //ListBox l1 = new ListBox();
            //l1.ItemsSource = cols;
            // t1.Content = l1;
            InitializeComponent();
        }

        public class Table : INotifyPropertyChanged
        {
            public string header { get; set; }
            public List<string> columns { get; set; }

            public Table()
            {
                header = "Header";
                columns = new List<string>();
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
