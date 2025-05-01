// <summary>
//   <para>
//     Small piece of code that defines a cell and stores the information within a cell.
//   </para>
// <date> October 12th, 2024 </date>
// <authors> bananathrowingmachine </authors>
// </summary>

// ReSharper disable once CheckNamespace
namespace Spreadsheet.Spreadsheet;

using Formula;
using System.Text.Json.Serialization;

public partial class Spreadsheet
{
    private class Cell
    {
        /// <summary>
        ///     Stores the method that returns the cell value
        /// </summary>
        private readonly Func<string, object> _getCellValue;

        /// <summary>
        ///     Builds a cell that takes in an object and also a way to find variables if it happens to be a formula
        /// </summary>
        /// <param name="content"></param>
        /// <param name="getCellValue"></param>
        public Cell(object content, Func<string, object> getCellValue)
        {
            Content = content;
            _getCellValue = getCellValue;
            Value = content.GetType() == typeof(Formula) ? ((Formula)content).Evaluate(Lookup) : content;
            if (content is Formula f)
                StringForm = "=" + f;
            else if (content is double d)
                StringForm = "" + d;
            else
                StringForm = (string)content;
        }

        /// <summary>
        ///     Default constructor for cells, should ONLY BE USED by the JSON deserializer or else it will create a broken cell
        /// </summary>
        public Cell()
        {
            Content = string.Empty;
            _getCellValue = a => a.Length;
            Value = string.Empty;
            StringForm = string.Empty;
        }

        /// <summary>
        ///     Stores the content of the cell as a property, and modifies the value whenever it is set
        /// </summary>
        [JsonIgnore]
        public object Content { get; }

        /// <summary>
        ///     The lookup method which is passed to Evaluate()
        /// </summary>
        /// <param name="name">Name of the cell to lookup</param>
        /// <returns>The number value of the cell</returns>
        /// <exception cref="ArgumentException">If the cell isn't a number (either FormulaError or string), the method will throw</exception>
        private double Lookup(string name)
        {
            var value = _getCellValue(name);
            if (value is double d)
                return d;
            throw new ArgumentException("Not a number");
        }

        /// <summary>
        ///     Stores the value of the cell, either it's content or the solution to the formula
        /// </summary>
        [JsonIgnore]
        public object Value { get; private set; }

        /// <summary>
        ///     Recalculates the value when needed
        /// </summary>
        public void Recalculate()
        {
            Value = Content.GetType() == typeof(Formula) ? ((Formula)Content).Evaluate(Lookup) : Content;
        }

        [JsonInclude] public string StringForm { get; private set; }
    }
}
