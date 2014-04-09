using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;

namespace SQLVisualBuilder
{
    public partial class Form2 : Form
    {
        QueryBuilder query { get; set; }
        public Form2(QueryBuilder q)
        {
            query = q;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string t1 = query.GetTables()[0].LeftColumn;
            List<string> columns = new List<string>();
            foreach (QueryBuilder.ColumnInfo i in query.GetColumns())
                columns.Add(i.Name);

            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            HostingWpfUserControlInWf.UserControl1 uc = new HostingWpfUserControlInWf.UserControl1("ABCD", columns);
            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = uc;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);
        }
    }
}
