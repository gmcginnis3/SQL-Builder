using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Collections;

namespace SQLVisualBuilder
{
    static class Program
    {
        static String dbConnection;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            dbConnection = "Data Source=C:\\Users\\Geoff\\Desktop\\ADBMS Project\\SQLVisualBuilder\\Olympics.sqlite";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            DataTable tables = Query("SELECT * FROM sqlite_master WHERE type='table';");
            ArrayList tableNames = new ArrayList();
            foreach(DataRow row in tables.Rows)
            {
                tableNames.Add(row["tbl_name"]);   
            }
            form1.setData(tableNames);
            
            Application.Run(form1);

        }

        public static DataTable Query(String sql)
        {
            DataTable table = new DataTable();
            try
            {
                SQLiteConnection connection = new SQLiteConnection(dbConnection);
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = sql;
                SQLiteDataReader reader = command.ExecuteReader();
                table.Load(reader);
                reader.Close();
                connection.Close();
            }
            catch (IOException e)
            {
                Console.Write(e.Message);
            }

            return table;
        }
    }
}
