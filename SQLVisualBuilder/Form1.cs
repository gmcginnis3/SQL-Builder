using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SQLVisualBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TabPage page = tabControl1.SelectedTab;
            if (!page.HasChildren)
                return;
            DataGridView dgv = page.Controls.Find("dataGrid", true)[0] as DataGridView;
            DataGridViewSelectedColumnCollection selected = dgv.SelectedColumns;
            textBox1.Text = selected.Count.ToString();

            QueryBuilder builder = new QueryBuilder();
            builder.QueryType = QueryTypes.Select;
            builder.AddTable(page.Text);
            if (selected.Count > 0)
            {
                foreach (DataGridViewColumn col in selected)
                {
                    builder.AddColumn(col.HeaderText);
                }
            }
            else
            {
                builder.AddColumn("*");
            }

            textBox1.Text += builder.ToString();
        }

        public void setData(ArrayList items)
        {
            //tabControl1.Parent = splitContainer1.Panel2;
            foreach (string table in items)
            {
                TabPage tp = new TabPage(table);
                tabControl1.TabPages.Add(tp);
                DataGridView dgv = new DataGridView();
                dgv.Name = "dataGrid";
                dgv.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left); ;
                dgv.Dock = DockStyle.Fill;
                dgv.AutoGenerateColumns = true;
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.DataSource = Program.Query("SELECT * FROM " + table);
                dgv.Parent = tp;
                /*foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
                dgv.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;*/
            }
        }

    }
}
