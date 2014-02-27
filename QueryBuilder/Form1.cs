//////////////////////////////////////////////////////////////////////////////
// This source code and all associated files and resources are copyrighted by
// the author(s). This source code and all associated files and resources may
// be used as long as they are used according to the terms and conditions set
// forth in The Code Project Open License (CPOL), which may be viewed at
// http://www.blackbeltcoder.com/Legal/Licenses/CPOL.
//
// Copyright (c) 2011 Jonathan Wood
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestQueryBuilder
{
	public partial class Form1 : Form
	{
		private delegate void QueryDemo();

		private class QueryInfo
		{
			public string Title { get; set; }
			public QueryDemo Demo { get; set; }
		}

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// Populate queries combo box
			cboQueries.Items.Add(new QueryInfo() { Title = "Select Query", Demo = SelectDemo });
			cboQueries.Items.Add(new QueryInfo() { Title = "Select Query 2", Demo = Select2Demo });
			cboQueries.Items.Add(new QueryInfo() { Title = "Select Query 3", Demo = Select3Demo });
			cboQueries.Items.Add(new QueryInfo() { Title = "Insert Query", Demo = InsertDemo });
			cboQueries.Items.Add(new QueryInfo() { Title = "Update Query", Demo = UpdateDemo });
			cboQueries.Items.Add(new QueryInfo() { Title = "Delete Query", Demo = DeleteDemo });
			cboQueries.SelectedIndex = 0;
		}

		private void btnGo_Click(object sender, EventArgs e)
		{
			QueryInfo info = cboQueries.SelectedItem as QueryInfo;
			if (info != null)
				info.Demo();
		}

		private void SelectDemo()
		{
			QueryBuilder builder = new QueryBuilder();
			builder.AddTable("Contacts");
			txtQuery.Text = builder.ToString();
		}

		private void Select2Demo()
		{
			QueryBuilder builder = new QueryBuilder();
			builder.AddColumn("Name");
			builder.AddColumn("City");
			builder.AddColumn("State");
			builder.AddColumn("Zip");
			builder.AddTable("Contacts");
			builder.AddCondition("[State] = 'UT'");
			builder.AddCondition("[State] = 'CA'", ConditionOperators.Or);
			builder.AddSortColumn("Name");
			txtQuery.Text = builder.ToString();
		}

		private void Select3Demo()
		{
			QueryBuilder builder = new QueryBuilder();
			builder.QueryType = QueryTypes.Select;
			builder.AddColumn("ID", "Contacts", "ContactID");
			builder.AddColumn("City", "Contacts");
			builder.AddColumn("State", "Contacts");
			builder.AddColumn("Zip", "Contacts");
			builder.AddColumn("Name", "Company", "CompanyName");
			builder.AddTable("Contacts");
			builder.AddTable("Contacts", "CompanyID", "Companies", "ID", JoinTypes.InnerJoin);
			builder.AddSortColumn("Name", "Companies", SortOrder.Descending);
			builder.AddSortColumn("Name", "Contact", SortOrder.Ascending);
			txtQuery.Text = builder.ToString();
		}

		private void InsertDemo()
		{
			QueryBuilder builder = new QueryBuilder();
			builder.QueryType = QueryTypes.Insert;
			builder.AddNameValuePair("Name", "'Bill'");
			builder.AddNameValuePair("City", "'Salt Lake City'");
			builder.AddNameValuePair("State", "'UT'");
			builder.AddNameValuePair("Zip", "'84084'");
			builder.AddTable("Contacts");
			txtQuery.Text = builder.ToString();
		}

		private void UpdateDemo()
		{
			QueryBuilder builder = new QueryBuilder();
			builder.QueryType = QueryTypes.Update;
			builder.AddNameValuePair("Name", "'Bill'");
			builder.AddNameValuePair("City", "'Salt Lake City'");
			builder.AddNameValuePair("State", "'UT'");
			builder.AddNameValuePair("Zip", "'84084'");
			builder.AddTable("Contacts");
			builder.AddCondition("[ID] = 123");
			txtQuery.Text = builder.ToString();
		}

		private void DeleteDemo()
		{
			QueryBuilder builder = new QueryBuilder();
			builder.QueryType = QueryTypes.Delete;
			builder.AddTable("Contacts");
			builder.AddCondition("[Name] = 'Bill'");
			builder.AddCondition("[Name] = 'Bob'", ConditionOperators.Or);
			txtQuery.Text = builder.ToString();
		}
	}
}
