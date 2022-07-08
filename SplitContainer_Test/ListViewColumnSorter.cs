using CloudProjectClient;
using RESTAPI;
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
    public int Compare(FileSystemStructureFolder x, FileSystemStructureFile y)
    {
        int compareResult = 1;
        switch (ColumnToSort)
        {
            case 0:
                compareResult = ObjectCompare.Compare(x.FolderName, y.FileName);
                break;
            case 1:
                compareResult = 1;
                break;
            case 2:
                compareResult = -1;
                break;
            case 3:
                compareResult = x.FolderEditTime.CompareTo(y.FileEditTime);
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

    public int Compare(FileSystemStructureFolder x, FileSystemStructureFolder y)
    {
        int compareResult = 0;
        switch (ColumnToSort)
        {
            case 0:
                compareResult = ObjectCompare.Compare(x.FolderName, y.FolderName);
                break;
            case 3:
                compareResult = x.FolderEditTime.CompareTo(y.FolderEditTime);
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

    public int Compare(FileSystemStructureFile x, FileSystemStructureFile y)
    {
        int compareResult = 0;
        switch (ColumnToSort)
        {
            case 0:
                compareResult = ObjectCompare.Compare(x.FileName, y.FileName);
                break;
            case 2:
                compareResult = x.FileLenght.CompareTo(y.FileLenght);
                break;
            case 3:
                compareResult = x.FileEditTime.CompareTo(y.FileEditTime);
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

    public int Compare(WindowsFolderTag x, WindowsFolderTag y)
    {
        int compareResult = 0;
        switch (ColumnToSort)
        {
            case 0:
                compareResult = ObjectCompare.Compare(x.Name, y.Name);
                break;
            case 3:
                compareResult = x.LastEditTime.CompareTo(y.LastEditTime);
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

    public int Compare(WindowsFolderTag x, WindowsFileTag y)
    {
        int compareResult = 1;
        switch (ColumnToSort)
        {
            case 0:
                compareResult = ObjectCompare.Compare(x.Name, y.Name);
                break;
            case 1:
                compareResult = 1;
                break;
            case 2:
                compareResult = -1;
                break;
            case 3:
                compareResult = x.LastEditTime.CompareTo(y.LastEditTime);
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


    public int Compare(WindowsFileTag x, WindowsFileTag y)
    {
        int compareResult = 0;
        switch (ColumnToSort)
        {
            case 0:
                compareResult = ObjectCompare.Compare(x.Name, y.Name);
                break;
            case 2:
                compareResult = x.Length.CompareTo(y.Length);
                break;
            case 3:
                compareResult = x.LastEditTime.CompareTo(y.LastEditTime);
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


    public int Compare(ListViewItem x, ListViewItem y)
    {
        object DataTransferX = x.Tag;
        object DataTransferY = y.Tag;

        if (DataTransferX.GetType() == typeof(FileSystemStructureFolder) && DataTransferY.GetType() == typeof(FileSystemStructureFolder))
            return Compare((FileSystemStructureFolder)DataTransferX, (FileSystemStructureFolder)DataTransferY);

        if (DataTransferX.GetType() == typeof(FileSystemStructureFile) && DataTransferY.GetType() == typeof(FileSystemStructureFile))
            return Compare((FileSystemStructureFile)DataTransferX, (FileSystemStructureFile)DataTransferY);

        if (DataTransferX.GetType() == typeof(FileSystemStructureFolder) && DataTransferY.GetType() == typeof(FileSystemStructureFolder))
            return Compare((FileSystemStructureFolder)DataTransferX, (FileSystemStructureFile)DataTransferY);

        if (DataTransferX.GetType() == typeof(WindowsFolderTag) && DataTransferY.GetType() == typeof(WindowsFolderTag))
            return Compare((WindowsFolderTag)DataTransferX, (WindowsFolderTag)DataTransferY);

        if (DataTransferX.GetType() == typeof(WindowsFolderTag) && DataTransferY.GetType() == typeof(WindowsFileTag))
            return Compare((WindowsFolderTag)DataTransferX, (WindowsFileTag)DataTransferY);

        if (DataTransferX.GetType() == typeof(WindowsFileTag) && DataTransferY.GetType() == typeof(WindowsFileTag))
            return Compare((WindowsFileTag)DataTransferX, (WindowsFileTag)DataTransferY);

        return 0;
    }

    public int Compare(object x, object y)
    {
        if (x.GetType() == typeof(ListViewItem) && y.GetType() == typeof(ListViewItem))
            return Compare((ListViewItem)x, (ListViewItem)y);


        return 0;
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