using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace SQLVisualBuilder
{
    public partial class Form1 : Form
    {
        QueryBuilder builder;
        Dictionary<string, List<string>> primaryKeys = new Dictionary<string,List<string>>();

        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
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
            int tabCount = tabControl1.TabCount;
            int pno = 0;
            TabPage[] pages = new TabPage[tabCount];
            for(int i=0;i<tabCount;i++)
            {
                TabPage mYpage = tabControl1.TabPages[i];
                if (!mYpage.HasChildren)
                    return;
                DataGridView mYdgv = mYpage.Controls.Find("dataGrid", true)[0] as DataGridView;
                DataGridViewSelectedCellCollection mYselected = mYdgv.SelectedCells;
                if (mYselected.Count > 0)
                {
                    pages[pno] = mYpage;     //More than one tables selected; create seperate tabs for each
                    pno++;
                }
            }

            if (pno==0) //Nothing selected
                textBox1.Text = "No cells selected.";
            else if (pno == 1) //single table
            {
                bool queryOk = true;
                TabPage page = pages[0];
                if (!page.HasChildren)
                    return;
                DataGridView dgv = page.Controls.Find("dataGrid", true)[0] as DataGridView;
                DataGridViewSelectedCellCollection selected = dgv.SelectedCells;
                
                //check if query is ok in terms of same columns selected 
                //queryOk = checkCellsForValidity(selected);
                if (queryOk == false)
                    textBox1.Text = "No valid query!";
                else
                {
                    builder = new QueryBuilder();
                    builder.QueryType = QueryTypes.Select;
                    builder.AddTable(page.Text);
                    if (selected.Count > 0)
                    {
                        HashSet<string> columns = new HashSet<string>();
                        foreach (DataGridViewCell cell in selected)
                        {
                            columns.Add(cell.OwningColumn.HeaderText);
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

                    DataSet_Apriori apriori = new DataSet_Apriori();
                    List<HashSet<string>> where = apriori.getAssocValues(selected, "");
                    string rangeCondition = "";

                    foreach (HashSet<string> s in where)
                    {
                        if (s.Count == 1)
                        {
                            builder.AddCondition(s.ElementAt(0).Replace("//", " = "));
                        }
                        else
                        {
                            if (s.ElementAt(0).Contains("/"))
                            {
                                builder.AddCondition(s.ElementAt(0).Replace("/", ">"));
                                builder.AddCondition(s.ElementAt(1).Replace("/", "<"));
                            }
                            else
                            {
                                rangeCondition += "(" + s.ElementAt(0).Replace("|", ">") + " AND " + s.ElementAt(1).Replace("|", "<") + ") OR ";
                            }
                        }
                    }
                    if (rangeCondition.Length > 0)
                    {
                        if (rangeCondition.EndsWith("OR "))
                            rangeCondition = rangeCondition.Substring(0, rangeCondition.Length - 4);
                        builder.AddCondition(rangeCondition);
                    }

                    textBox1.Text = builder.ToString();
                    button2.Enabled = true;
                }
            }
            else //deal with joins
            {
                DataGridViewSelectedCellCollection[] selected = new DataGridViewSelectedCellCollection[pno];
                bool joinOk = false;
                List<HashSet<string>> allColumns = new List<HashSet<string>>();
                IEnumerable<string> both = new List<string>();
                //create list of hash sets of different tables and see which columns in each table are selected
                for (int i = 0; i < pno ; i++)
                {
                    TabPage page = pages[i];
                    if (!page.HasChildren)
                        return;
                    DataGridView dgv = page.Controls.Find("dataGrid", true)[0] as DataGridView;
                    selected[i] = dgv.SelectedCells;
                    if (selected[i].Count > 0)
                    {
                        HashSet<string> columns = new HashSet<string>();
                        foreach (DataGridViewCell cell in selected[i])
                            columns.Add(cell.OwningColumn.HeaderText);
                        allColumns.Add(columns);
                    }
                }
                //Check for validity of join based on join attribute
                if (allColumns.Count > 1)
                //Two table join
                {
                    both = allColumns[0].Intersect(allColumns[1]);
                    if (both.Count() > 0)
                        joinOk = true;
                }
                else
                //Multiple table join
                { 
                    /*.......................................................................................................................................*/
                }
                //Perform join or show error message
                if (joinOk == true)
                {
                    builder = new QueryBuilder();
                    builder.QueryType = QueryTypes.Select;
                    builder.AddTable(pages[0].Text);
                    builder.AddTable(pages[0].Text, both.ToList()[0], pages[1].Text, both.ToList()[0], JoinTypes.Join);
                    if (allColumns[0].Count == (pages[0].Controls.Find("dataGrid", true)[0] as DataGridView).ColumnCount)
                    {
                        builder.AddColumn("*");
                    }
                    else
                    {
                        foreach (string col in allColumns[0])
                        {
                            if ((pages[0].Controls.Find("dataGrid", true)[0] as DataGridView).Columns.Contains(col))
                                builder.AddColumn(col, pages[0].Text);
                            else
                                builder.AddColumn(col, pages[1].Text);
                        }
                    }

                    DataSet_Apriori apriori = new DataSet_Apriori();
                    List<HashSet<string>> where = new List<HashSet<string>>();
                    int index = 0;
                    foreach (DataGridViewSelectedCellCollection cells in selected)
                    {
                        foreach (HashSet<string> set in apriori.getAssocValues(cells,pages[index].Text+"." ))
                            where.Add(set);
                        index++;
                    }
                    string rangeCondition = "";
                    foreach (HashSet<string> s in where)
                    {
                        if (s.Count == 1)
                        {
                            builder.AddCondition(s.ElementAt(0).Replace("//", " = "));
                        }
                        else
                        {
                            if (s.ElementAt(0).Contains("/"))
                            {
                                builder.AddCondition(s.ElementAt(0).Replace("/", ">"));
                                builder.AddCondition(s.ElementAt(1).Replace("/", "<"));
                            }
                            else
                            {
                                rangeCondition += "(" + s.ElementAt(0).Replace('|', '>') + " AND " + s.ElementAt(1).Replace('|', '<') + ") OR ";
                            }
                        }
                    }
                    if (rangeCondition.Length > 0)
                    {
                        if (rangeCondition.EndsWith("OR "))
                            rangeCondition = rangeCondition.Substring(0, rangeCondition.Length - 4);
                        builder.AddCondition(rangeCondition);
                    }

                    textBox1.Text = builder.ToString();
                    button2.Enabled = true;
                }
                else
                {
                    textBox1.Text = "No valid join query! Change your selection.";
                }
            }
        }

        //sets up and fills the table with data from the database
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
                DataTable t = Program.Query("SELECT * FROM " + table);
                dgv.DataSource = t;
                dgv.Parent = tp;
                dgv.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                dgv.ClearSelection();

                primaryKeys.Add(table, new List<string>());
                foreach (DataColumn key in t.PrimaryKey)
                    primaryKeys[table].Add(key.ColumnName);
            }

        }

        /*public bool checkCellsForValidity(DataGridViewSelectedCellCollection selected)
        {
            int n;
            //Set n= number of cells in first row found 
                if(selected.Count%n==0)
                    return true;
                else
                    return false;
             
        }*/

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(builder);
            form2.Show();

        }
    }
}
