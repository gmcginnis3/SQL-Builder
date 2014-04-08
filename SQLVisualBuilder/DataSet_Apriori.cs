using System;
using System.Linq;
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
                    if (((float)items[col][val] / selected_row_count) >= min_support)
                    {
                        HashSet<string> tempSet = new HashSet<string>();
                        tempSet.Add(col + "//" + val);
                        candidates.Add(tempSet);
                    }
                }
            }

            foreach (string col in items.Keys)
            {
                int[] values = new int[items[col].Keys.Count];
                string[] strings = new string[items[col].Keys.Count];
                items[col].Keys.CopyTo(strings, 0);
                try
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = Convert.ToInt32(strings[i]);
                    }
                }
                catch (FormatException e)
                {
                    //The column does not conatin ints
                }
                finally
                {
                    List<HashSet<int>> range = getAssocRange(new List<int>(values), values.Length);
                    if (range != null && range.Count > 1)
                    {
                        foreach (HashSet<int> set in range)
                        {
                            HashSet<string> temp = new HashSet<string>();
                            foreach (int x in set)
                            {
                                temp.Add(col + "||" + x.ToString());
                            }
                            candidates.Add(temp);
                        }
                    }
                    else if (range != null && range.Count == 1)
                    {
                        HashSet<string> temp = new HashSet<string>();
                        foreach (int x in range.Last())
                        {
                            temp.Add(col + "//" + x.ToString());
                        }
                        candidates.Add(temp);
                    }
                }
            }

            return candidates;
        }

        public List<HashSet<int>> getAssocRange(List<int> values, int totalCount)
        {
            int binCount = 5;
            int max = values.Max();
            int min = values.Min();

            List<int>[] bins = new List<int>[binCount];
            for (int i = 0; i < binCount; i++)
                bins[i] = new List<int>();

            int binSize = (max - min) / binCount;
            if (binSize == 0)
                return null;
            int index = 0;
            foreach (int val in values)
            {
                index = (val - min) / binSize;
                if (index >= binCount)
                    index = binCount - 1;
                
                bins[index].Add(val);
            }

            List<HashSet<int>> result = new List<HashSet<int>>(); ;
            for (int j = 0; j < binCount; j++)
            {
                if (bins[j].Count != 0)
                {
                    result.Add(new HashSet<int>());
                    result.Last().Add(min + j * binSize);

                    for (int k = j + 1; k < binCount && bins[k].Count > 0; k++)
                    {
                        j++;
                    }

                    result.Last().Add(min + (j + 1) * binSize);
                }
            }
            return result;
        }
    }
}
