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
using System.Linq;
using System.Text;

namespace TestQueryBuilder
{
	public enum QueryTypes
	{
		Select,
		Insert,
		Update,
		Delete
	}

	public enum JoinTypes
	{
		InnerJoin,
		LeftJoin,
		RightJoin,
		OuterJoin
	}

	public enum ConditionOperators
	{
		And,
		Or
	}

	public enum SortOrder
	{
		Ascending,
		Descending
	}

	/// <summary>
	/// Class to construct SQL queries
	/// </summary>
	class QueryBuilder
	{
		/// <summary>
		/// Class to track column information
		/// </summary>
		private class ColumnInfo
		{
			public string Name { get; set; }
			public string Alias { get; set; }
			public string TableName { get; set; }

			public override string ToString()
			{
				StringBuilder builder = new StringBuilder();
				if (TableName != null)
					builder.AppendFormat("[{0}].", TableName);
				builder.AppendFormat("[{0}]", Name);
				if (Alias != null)
					builder.AppendFormat(" AS [{0}]", Alias);
				return builder.ToString();
			}
		}

		/// <summary>
		/// Class to track table information
		/// </summary>
		private class TableInfo
		{
			public string Name { get; set; }
			public JoinTypes JoinType { get; set; }
			public string LeftTable { get; set; }
			public string LeftColumn { get; set; }
			public string RightColumn { get; set; }

			public override string ToString()
			{
				return ToString(true);
			}

			public string ToString(bool isFirstTable)
			{
				// First table has no join
				if (isFirstTable)
					return String.Format("[{0}]", Name);

				// Table with join
				StringBuilder builder = new StringBuilder();
				switch (JoinType)
				{
					case JoinTypes.InnerJoin:
						builder.Append(" INNER JOIN");
						break;
					case JoinTypes.LeftJoin:
						builder.Append(" LEFT OUTER JOIN");
						break;
					case JoinTypes.RightJoin:
						builder.Append(" RIGHT OUTER JOIN");
						break;
					case JoinTypes.OuterJoin:
						builder.Append(" FULL OUTER JOIN");
						break;
				}
				builder.AppendFormat(" [{0}] ON [{1}].[{2}] = [{0}].[{3}]",
					Name, LeftTable, LeftColumn, RightColumn);
				return builder.ToString();
			}
		}

		/// <summary>
		/// Class to track WHERE condition information
		/// </summary>
		private class ConditionInfo
		{
			public string Text { get; set; }
			public ConditionOperators Operator { get; set; }

			public override string ToString()
			{
				return ToString(true);
			}

			public string ToString(bool isFirstCondition)
			{
				// First condition has no logical operator
				if (isFirstCondition)
					return String.Format(" {0}", Text);

				// Condition with logical operator
				StringBuilder builder = new StringBuilder();
				switch (Operator)
				{
					case ConditionOperators.And:
						builder.Append(" AND");
						break;
					case ConditionOperators.Or:
						builder.Append(" OR");
						break;
				}
				builder.AppendFormat(" {0}", Text);
				return builder.ToString();
			}
		}

		/// <summary>
		/// Class to track sort information
		/// </summary>
		private class SortInfo
		{
			public string Name { get; set; }
			public string Table { get; set; }
			public SortOrder Direction { get; set; }

			public override string ToString()
			{
				return ToString(true);
			}

			public string ToString(bool isFirstColumn)
			{
				StringBuilder builder = new StringBuilder();
				if (isFirstColumn)
					builder.Append(" ");
				else
					builder.Append(", ");
				if (!String.IsNullOrEmpty(Table))
					builder.AppendFormat("[{0}].", Table);
				builder.AppendFormat("[{0}]", Name);
				if (Direction == SortOrder.Descending)
					builder.Append(" DESC");
				return builder.ToString();
			}
		}

		// Private members to track current settings
		private List<ColumnInfo> _columns = new List<ColumnInfo>();
		private List<KeyValuePair<string, string>> _nameValuePairs = new List<KeyValuePair<string, string>>();
		private List<TableInfo> _tables = new List<TableInfo>();
		private List<ConditionInfo> _conditions = new List<ConditionInfo>();
		private List<SortInfo> _sortColumns = new List<SortInfo>();

		// Type of query (SELECT, INSERT, UPDATE, DELETE)
		public QueryTypes QueryType { get; set; }

		/// <summary>
		/// Construction
		/// </summary>
		public QueryBuilder()
		{
			Reset();
		}

		/// <summary>
		/// Restores this instance to default settings.
		/// </summary>
		public void Reset()
		{
			QueryType = QueryTypes.Select;
			_columns.Clear();
			_nameValuePairs.Clear();
			_tables.Clear();
			_tables.Clear();
		}

		/// <summary>
		/// Adds a column for a SELECT query
		/// </summary>
		/// <param name="colName">Column name</param>
		public void AddColumn(string colName)
		{
			AddColumn(colName, null, null);
		}

		/// <summary>
		/// Adds a column for a SELECT query
		/// </summary>
		/// <param name="colName">Column name</param>
		/// <param name="tableName">Name of table</param>
		public void AddColumn(string colName, string tableName)
		{
			AddColumn(colName, tableName, null);
		}

		/// <summary>
		/// Adds a column for a SELECT query
		/// </summary>
		/// <param name="colName">Column name</param>
		/// <param name="tableName">Name of table</param>
		/// <param name="alias">Column alias name</param>
		public void AddColumn(string colName, string tableName, string alias)
		{
			_columns.Add(new ColumnInfo() { Name = colName, TableName = tableName, Alias = alias });
		}

		/// <summary>
		/// Adds a column name/value pair for INSERT or UPDATE queries
		/// </summary>
		/// <param name="name">Column name</param>
		/// <param name="value">Column value</param>
		public void AddNameValuePair(string name, string value)
		{
			_nameValuePairs.Add(new KeyValuePair<string, string>(name, value));
		}

		/// <summary>
		/// Adds an initial or only table to the query.
		/// </summary>
		/// <param name="tableName"></param>
		public void AddTable(string tableName)
		{
			if (_tables.Count > 0)
				throw new Exception("Must supply JOIN parameters in additional tables");
			AddTable(null, null, tableName, null, JoinTypes.InnerJoin);
		}

		/// <summary>
		/// Adds a subsequent table to the query
		/// </summary>
		/// <param name="leftTable">Name of table on left side of join</param>
		/// <param name="leftColumn">Name of column on left side of join</param>
		/// <param name="newTable">Name of table being added</param>
		/// <param name="rightColumn">Name of column on right side of join</param>
		/// <param name="type">Join type</param>
		public void AddTable(string leftTable, string leftColumn, string newTable, string rightColumn, JoinTypes type)
		{
			_tables.Add(new TableInfo()
				{
					Name = newTable,
					JoinType = type,
					LeftTable = leftTable,
					LeftColumn = leftColumn,
					RightColumn = rightColumn
				});
		}

		/// <summary>
		/// Adds a WHERE condition to the current query
		/// </summary>
		/// <param name="text">Text of WHERE condition</param>
		public void AddCondition(string text)
		{
			AddCondition(text, ConditionOperators.And);
		}

		/// <summary>
		/// Adds a WHERE condition to the current query
		/// </summary>
		/// <param name="text">Text of WHERE condition</param>
		/// <param name="op">Logical operator joining this condition with previous one</param>
		public void AddCondition(string text, ConditionOperators op)
		{
			_conditions.Add(new ConditionInfo() { Text = text, Operator = op });
		}

		/// <summary>
		/// Adds an ORDER BY column to the current query
		/// </summary>
		/// <param name="colName">Name of column used for sorting</param>
		public void AddSortColumn(string colName)
		{
			AddSortColumn(colName, SortOrder.Ascending);
		}

		/// <summary>
		/// Adds an ORDER BY column to the current query
		/// </summary>
		/// <param name="colName">Name of column used for sorting</param>
		/// <param name="direction">Sort direction</param>
		public void AddSortColumn(string colName, SortOrder direction)
		{
			AddSortColumn(colName, null, direction);
		}

		/// <summary>
		/// Adds an ORDER BY column to the current query
		/// </summary>
		/// <param name="colName">Name of column used for sorting</param>
		/// <param name="tableName">Name of table this column is part of</param>
		/// <param name="direction">Sort direction</param>
		public void AddSortColumn(string colName, string tableName, SortOrder direction)
		{
			_sortColumns.Add(new SortInfo() { Name = colName, Table = tableName, Direction = direction });
		}

		/// <summary>
		/// Returns a query string using the current settings
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			switch (QueryType)
			{
				case QueryTypes.Select:
					builder.Append("SELECT");
					builder.AppendFormat(" {0}", BuildColumnString());
					builder.AppendFormat(" FROM {0}", BuildTableString());
					builder.Append(BuildConditionString());
					builder.Append(BuildSortString());
					break;

				case QueryTypes.Insert:
					builder.Append("INSERT");
					builder.AppendFormat(" INTO {0}", BuildTableString(true));
					builder.AppendFormat(" ({0}) VALUES ({1})",
						BuildPairNameStrings(_nameValuePairs),
						BuildPairValueString(_nameValuePairs));
					break;

				case QueryTypes.Update:
					builder.Append("UPDATE");
					builder.AppendFormat(" {0} SET ", BuildTableString(true));
					builder.Append(BuildPairNameValueString(_nameValuePairs));
					builder.Append(BuildConditionString());
					break;

				case QueryTypes.Delete:
					builder.Append("DELETE");
					builder.AppendFormat(" FROM {0}", BuildTableString(true));
					builder.Append(BuildConditionString());
					break;
			}
			return builder.ToString();
		}

		/// <summary>
		/// Constructs a string with list of columns
		/// </summary>
		/// <returns></returns>
		protected string BuildColumnString()
		{
			if (_columns.Count == 0)
				return "*";

			StringBuilder builder = new StringBuilder();
			foreach (ColumnInfo info in _columns)
			{
				if (builder.Length > 0)
					builder.Append(", ");
				builder.Append(info.ToString());
			}
			return builder.ToString();
		}

		/// <summary>
		/// Constructs a string with list of tables
		/// </summary>
		/// <returns></returns>
		protected string BuildTableString()
		{
			return BuildTableString(false);
		}

		/// <summary>
		/// Constructs a string with list of tables
		/// </summary>
		/// <param name="singleTable">Set to true if all but first table should be ignored</param>
		/// <returns></returns>
		protected string BuildTableString(bool singleTable)
		{
			if (_tables.Count == 0)
				throw new Exception("No table specified");

			if (singleTable)
				return _tables[0].ToString(true);

			StringBuilder builder = new StringBuilder();
			foreach (TableInfo info in _tables)
			{
				builder.Append(info.ToString(builder.Length == 0));
			}
			return builder.ToString();
		}

		/// <summary>
		/// Construcuts a string with a list of WHERE conditions
		/// </summary>
		/// <returns></returns>
		protected string BuildConditionString()
		{
			if (_conditions.Count == 0)
				return String.Empty;

			StringBuilder builder = new StringBuilder();
			foreach (ConditionInfo info in _conditions)
			{
				builder.Append(info.ToString(builder.Length == 0));
			}
			builder.Insert(0, " WHERE");
			return builder.ToString();
		}

		/// <summary>
		/// Constructs a string with a list of ORDER BY columns
		/// </summary>
		/// <returns></returns>
		protected string BuildSortString()
		{
			if (_sortColumns.Count == 0)
				return String.Empty;

			StringBuilder builder = new StringBuilder();
			foreach (SortInfo info in _sortColumns)
			{
				builder.Append(info.ToString(builder.Length == 0));
			}
			builder.Insert(0, " ORDER BY");
			return builder.ToString();
		}

		/// <summary>
		/// Constructs a list of names from name/value pairs
		/// </summary>
		/// <param name="pairs">Name/value pairs from which to construct string</param>
		/// <returns></returns>
		protected string BuildPairNameStrings(List<KeyValuePair<string, string>> pairs)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var pair in pairs)
			{
				if (builder.Length > 0)
					builder.Append(", ");
				builder.AppendFormat("[{0}]", pair.Key);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Constructs a list of values from name/values pairs
		/// </summary>
		/// <param name="pairs">Name/value pairs from which to construct string</param>
		/// <returns></returns>
		protected string BuildPairValueString(List<KeyValuePair<string, string>> pairs)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var pair in pairs)
			{
				if (builder.Length > 0)
					builder.Append(", ");
				builder.Append(pair.Value);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Constructs a list of name/values from name/values pairs
		/// </summary>
		/// <param name="pairs">Name/value pairs from which to construct string</param>
		/// <returns></returns>
		protected string BuildPairNameValueString(List<KeyValuePair<string, string>> pairs)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var pair in _nameValuePairs)
			{
				if (builder.Length > 0)
					builder.Append(", ");
				builder.AppendFormat("[{0}] = {1}", pair.Key, pair.Value);
			}
			return builder.ToString();
		}
	}
}
