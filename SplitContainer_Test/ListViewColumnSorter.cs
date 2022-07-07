using System;
using System.Collections;
using System.Windows.Forms;

/// <summary>
/// This class is an implementation of the 'IComparer' interface.
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int ColumnToSort;

    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>
    private SortOrder OrderOfSort;

    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// Class constructor. Initializes various elements
    /// </summary>
    public ListViewColumnSorter()
    {
        // Initialize the column to '0'
        ColumnToSort = 0;

        // Initialize the sort order to 'none'
        OrderOfSort = SortOrder.None;

        // Initialize the CaseInsensitiveComparer object
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// This method is inherited from the IComparer interface. It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public int Compare(object x, object y)
    {
        int compareResult;
        ListViewItem listviewX, listviewY;

        // Cast the objects to be compared to ListViewItem objects
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        switch (ColumnToSort)
        {
            case 0:
                if ((bool)listviewX.Tag != (bool)listviewY.Tag)
                {
                    if ((bool)listviewX.Tag)
                        compareResult = 1;
                    else
                        compareResult = -1;
                }
                else
                {
                    compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
                }
                break;
            case 1:
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
                break;
            case 2:
                string firstCellText = listviewX.SubItems[ColumnToSort].Text;
                string secondCellText = listviewY.SubItems[ColumnToSort].Text;

                if (firstCellText.Length == 0 && secondCellText.Length == 0)
                {
                    compareResult = 0;
                    break;
                }
                else if (firstCellText.Length == 0 || secondCellText.Length == 0)
                {
                    compareResult = firstCellText.Length == 0 ? -1 : 1;
                    break;
                }

                string[] firstText = firstCellText.Split(' ');
                string[] secondText = secondCellText.Split(' ');

                if (string.Equals(firstText[1], secondText[1]))
                {
                    compareResult = ObjectCompare.Compare(Int32.Parse(firstText[0]), Int32.Parse(secondText[0]));
                }
                else
                {
                    if (string.Equals(firstText[1], "ГБ"))
                    {
                        compareResult = 1;
                    }
                    else if (string.Equals(secondText[1], "ГБ"))
                    {
                        compareResult = -1;
                    }
                    else if (string.Equals(firstText[1], "МБ"))
                    {
                        compareResult = 1;
                    }
                    else if (string.Equals(secondText[1], "МБ"))
                    {
                        compareResult = -1;
                    }
                    else if (string.Equals(firstText[1], "КБ"))
                    {
                        compareResult = 1;
                    }
                    else if (string.Equals(secondText[1], "КБ"))
                    {
                        compareResult = -1;
                    }
                    else
                        compareResult = -1;
                }

                break;
            case 3:
                compareResult = DateTime.Parse(listviewX.SubItems[ColumnToSort].Text).CompareTo(DateTime.Parse(listviewY.SubItems[ColumnToSort].Text));
                break;
            default:
                compareResult = 0;
                break;
        }



        // Calculate correct return value based on object comparison
        if (OrderOfSort == SortOrder.Ascending)
        {
            // Ascending sort is selected, return normal result of compare operation
            return compareResult;
        }
        else if (OrderOfSort == SortOrder.Descending)
        {
            // Descending sort is selected, return negative result of compare operation
            return (-compareResult);
        }
        else
        {
            // Return '0' to indicate they are equal
            return 0;
        }
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn
    {
        set
        {
            ColumnToSort = value;
        }
        get
        {
            return ColumnToSort;
        }
    }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order
    {
        set
        {
            OrderOfSort = value;
        }
        get
        {
            return OrderOfSort;
        }
    }

}