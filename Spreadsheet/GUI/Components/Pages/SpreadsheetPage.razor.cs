// Made by bananathrowingmachine and [redacted], Oct 18 2024

using Spreadsheet.Formula;
using Spreadsheet.Spreadsheet;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace GUI.Client.Pages;

/// <summary>
/// Used to create the backing information used for the spreadsheet. Handles storing spreadsheet contents, values, and saving/loading files.
/// </summary>
public partial class SpreadsheetPage
{
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int Rows = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int Cols = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    /// Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set;  } = "Spreadsheet.sprd";

    /// <summary>
    /// Creates a spreadsheet used for saving cells and their dependencies.
    /// </summary>
    private Spreadsheet _spreadsheet = new();

    /// <summary>
    ///   <para> Gets or sets the value contained for all the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private object[,] CellsValueStore { get; set; } = new object[Rows, Cols];

    /// <summary>
    /// Used to record the currently selected cell. Defaults to A1.
    /// </summary>
    private string _selectedCell = "A1";

    /// <summary>
    /// Used to record the contents of the currently selected cell.
    /// </summary>
    private string _currentContents = string.Empty;

    /// <summary>
    /// Used to record the value of the currently selected cell.
    /// </summary>
    private object _currentValue = string.Empty;

    /// <summary>
    /// Used to refocus cursor from cells to the formula input box.
    /// </summary>
    private ElementReference _textArea;

    /// <summary>
    /// The currently selected row.
    /// </summary>
    private int _selectedRow;

    /// <summary>
    /// The currently selected column.
    /// </summary>
    private int _selectedCol;

    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked(int row, int col)
    {

        _selectedCell = Alphabet[col].ToString() + (row + 1);

        _selectedRow = row;
        _selectedCol = col;

        var content = _spreadsheet.GetCellContents(_selectedCell);
        string stringForm = content switch
        {
            Formula f => "=" + f,
            double d => "" + d,
            _ => (string)content
        };
        _currentContents = stringForm;
        _currentValue = CellsValueStore[row, col];

        _textArea.FocusAsync();
    }

    /// <summary>
    /// Used to handle when cell input occurs.
    /// </summary>
    /// <param name="e"> A change occurred in the input box. </param>
    private void CellContentChanged(ChangeEventArgs e)
    {
        // This uses the null forgiving (!) and coalescing (??)
        // operators to get either the value that was typed in,
        // or the empty string if it was null
        string data = e.Value!.ToString() ?? "";

        try
        {
            foreach (var cell in _spreadsheet.SetContentsOfCell(_selectedCell, data))
            {
                Object cellValue = _spreadsheet.GetCellValue(cell);

                if (cellValue is FormulaError)
                    CellsValueStore[int.Parse(cell[1..]) - 1, cell[0] - 'A'] = "Formula Error";
                else
                    CellsValueStore[int.Parse(cell[1..]) - 1, cell[0] - 'A'] = cellValue;
            }
            _currentContents = data;
        }
        catch (Exception ex)
        {
            if (ex is FormulaFormatException || ex is CircularException)
            {
                JSRuntime.InvokeVoidAsync("anExceptionOccurred");
                _currentContents = string.Empty;
                StateHasChanged();
                return;
            }
            throw;
        }
        
        _currentValue = CellsValueStore[_selectedRow, _selectedCol];
        _textArea.FocusAsync();
        StateHasChanged();

    }

    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        await JSRuntime.InvokeVoidAsync("downloadFile", FileSaveName,
            _spreadsheet.JsonStringData());
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
        try
        {
            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");

            if (eventArgs.FileCount != 1) return;
            var file = eventArgs.File;

            await using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            // fileContent will contain the contents of the loaded file
            var fileContent = await reader.ReadToEndAsync();

            _spreadsheet.ReplaceSpreadsheet(fileContent);
            
            Array.Clear(CellsValueStore, 0, CellsValueStore.Length);
            
            foreach (var cell in _spreadsheet.GetNamesOfAllNonemptyCells())
            {
                CellsValueStore[int.Parse(cell[1..]) - 1, cell[0] - 65] = _spreadsheet.GetCellValue(cell);
            }
            StateHasChanged();
        }
        catch (Exception e)
        {
            await JSRuntime.InvokeVoidAsync("fileLoadException");
            Debug.WriteLine("an error occurred while loading the file..." + e);
        }
    }
}
