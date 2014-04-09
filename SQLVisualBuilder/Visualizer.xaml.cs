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
using QuickGraph;
using GraphSharp.Controls;

namespace SQLVisualBuilder
{
    /// <summary>
    /// Interaction logic for Visualizer.xaml
    /// </summary>
    public partial class Visualizer : UserControl
    {
        QueryBuilder query { get; set; }
        
        TableGraph graph;

        public TableGraph GraphToVisualize
        {
            get { return graph; }
        }

        public Visualizer(QueryBuilder builder)
        {
            query = builder;
            Generate_Graph();
            InitializeComponent();
        }
        public void Generate_Graph()
        {
            graph = new TableGraph();
            List<Node> nodes = new List<Node>();
            List<QueryBuilder.TableInfo> tables = query.GetTables();
            List<QueryBuilder.ColumnInfo> columns = query.GetColumns();
            for (int i = 0; i < tables.Count; i++)
            {
                List<string> cols = new List<string>();
                for (int j = 0; j < columns.Count; j++)
                {
                    if (columns[j].TableName.Equals(tables[i].Name))
                        cols.Add(columns[j].Name);
                }
                Node temp = new Node(i, tables[i].LeftTable, cols);
                graph.AddVertex(temp);
                nodes.Add(temp);
                Console.WriteLine("_" + graph.VertexCount.ToString());
            }
            graph.AddEdge(new GraphEdge(tables[0].LeftColumn + "=" + tables[0].LeftColumn, nodes[0], nodes[1]));
            Console.WriteLine(".." + graph.EdgeCount.ToString());
        }
    }

    public class Node
    {
        public int id { get; set; }
        public string table { get; private set; }
        public List<string> columns { get; private set; }

        public Node(int ID, string tableName, List<string> cols)
        {
            id = ID;
            table = table;
            columns = cols;
        }
    }

    public class GraphEdge : Edge<Node>
    {
        public string ID { get; private set; }

        public GraphEdge(string id, Node source, Node target)
            : base(source, target)
        {
            ID = id;
        }
    }

    public class TableGraph : BidirectionalGraph<Node, GraphEdge>
    {
        public TableGraph() { }
    }

    public class TableGraphLayout : GraphLayout<Node, GraphEdge, TableGraph> { }
}
