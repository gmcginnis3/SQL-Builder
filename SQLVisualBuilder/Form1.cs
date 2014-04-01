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
            DataGridViewSelectedCellCollection selected = dgv.SelectedCells;
            HashSet<string> rows = new HashSet<string>();

            QueryBuilder builder = new QueryBuilder();
            builder.QueryType = QueryTypes.Select;
            builder.AddTable(page.Text);
            if (selected.Count > 0)
            {
                HashSet<string> columns = new HashSet<string>();
                foreach (DataGridViewCell cell in selected)
                {
                    columns.Add(cell.OwningColumn.HeaderText);
                    rows.Add(cell.OwningRow.Index.ToString());
                }
                if (columns.Count == dgv.ColumnCount)
                {
                    builder.AddColumn("*");
                }
                else
                {
                    foreach (string col in columns)
                    {
                        builder.AddColumn(col);
                    }
                }
            }
            else
            {
                builder.AddColumn("*");
            }

            if (rows.Count != dgv.RowCount || rows.Count == dgv.RowCount - 1)
            {
                DataSet_Apriori apriori = new DataSet_Apriori();
                List<HashSet<string>> where = apriori.getAssocValues(selected);

                foreach (HashSet<string> s in where)
                {
                    if (s.Count == 1)
                    {
                        builder.AddCondition(s.ElementAt(0).Replace("//", " = "));
                    }
                    else
                    {
                        builder.AddCondition(s.ElementAt(0).Replace("//", " > "));
                        builder.AddCondition(s.ElementAt(1).Replace("//", " < "));
                    }
                }
            }

            textBox1.Text = builder.ToString();
        }

        public void setData(ArrayList items)
        {
            foreach (string table in items)
            {
                TabPage tp = new TabPage(table);
                tabControl1.TabPages.Add(tp);
                DataGridView dgv = new DataGridView();
                dgv.Name = "dataGrid";
                dgv.ReadOnly = true;
                dgv.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left); ;
                dgv.Dock = DockStyle.Fill;
                dgv.AutoGenerateColumns = true;
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.DataSource = Program.Query("SELECT * FROM " + table);
                dgv.Parent = tp;
                dgv.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                dgv.ClearSelection();
            }

        }
    }
}
