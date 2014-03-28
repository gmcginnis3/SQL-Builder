using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

/*
 * This program uses a modified version of the apriori algortithm to determine commonalities among the selected cells.
 */
namespace SQLVisualBuilder
{
    public class DataSet_Apriori{

        public List<HashSet<string>> getAssocValues(DataGridViewSelectedCellCollection cells)
        {
            Dictionary<string, Dictionary<string, int>> items = new Dictionary<string, Dictionary<string, int>>();
            HashSet<int> rows = new HashSet<int>();

            /*
             * Analyze the support for each cell value
             */
            foreach (DataGridViewCell cell in cells)
            {
                if (items.ContainsKey(cell.OwningColumn.Name))
                {
                    if (items[cell.OwningColumn.Name].ContainsKey(cell.Value.ToString()))
                    {
                        items[cell.OwningColumn.Name][cell.Value.ToString()] += 1;
                    }
                    else
                    {
                        items[cell.OwningColumn.Name].Add(cell.Value.ToString(), 1);
                    }
                }
                else
                {
                    items.Add(cell.OwningColumn.Name, new Dictionary<string, int>());
                    items[cell.OwningColumn.Name].Add(cell.Value.ToString(), 1);
                }

                rows.Add(cell.OwningRow.Index);
            }

            /*
             * Find the frequent values and treat them as candidates
             */
            float selected_row_count = (float)rows.Count;
            float min_support = 1f;

            List<HashSet<string>> candidates = new List<HashSet<string>>();

            foreach(string col in items.Keys)
            {
                foreach (string val in items[col].Keys)
                {
                    Console.WriteLine(items[col][val].ToString() + "--" + selected_row_count.ToString());
                    if (((float)items[col][val] / selected_row_count) >= min_support)
                    {
                        HashSet<string> tempSet = new HashSet<string>();
                        tempSet.Add(col + "//" + val);
                        candidates.Add(tempSet);
                    }
                }
            }

            return candidates;
        }
    }
}
