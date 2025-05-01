// Written by bananathrowingmachine, Oct 12 2024

// ReSharper disable once CheckNamespace

using Spreadsheet.Formula;

namespace SpreadsheetTests;

using Spreadsheet.Spreadsheet;

/// <summary>
///     <para>
///         The following class shows the basics of how to use the MSTest framework,
///         including:
///     </para>
///     <list type="number">
///         <item> How to catch exceptions. </item>
///         <item> How a test of valid code should look. </item>
///     </list>
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonEmptyCells_BunchOfCells_ReturnsAllNames()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "5.0");
        ss.SetContentsOfCell("B1", "5.0");
        ss.SetContentsOfCell("F1", "5.0");
        ss.SetContentsOfCell("G3", "5.0");
        ss.SetContentsOfCell("AA2", "5.0");
        ss.SetContentsOfCell("B5", "5.0");
        ss.SetContentsOfCell("B6", "5.0");
        ss.SetContentsOfCell("H7", "5.0");
        Assert.IsTrue(
            new HashSet<string> { "A1", "B1", "F1", "G3", "AA2", "B5", "B6", "H7" }.SequenceEqual(
                ss.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonEmptyCells_VeryBigCells_ReturnsAllNames()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("AAA56778", "5.0");
        ss.SetContentsOfCell("ABCDEF121", "5.0");
        Assert.IsTrue(new HashSet<string> { "AAA56778", "ABCDEF121" }.SequenceEqual(ss.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonEmptyCells_NoCells_ReturnsAllNames()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.IsTrue(new HashSet<string>().SequenceEqual(ss.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonEmptyCells_LowercaseInputs_ReturnsAllNamesUppercase()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5.0");
        Assert.IsTrue(new HashSet<string> { "A1" }.SequenceEqual(ss.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_Number_ReturnsItself()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.IsTrue(new List<String> { "A1" }.SequenceEqual(ss.SetContentsOfCell("A1", "5.0")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_String_ReturnsItself()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.IsTrue(new List<String> { "A1" }.SequenceEqual(ss.SetContentsOfCell("A1", "textgoeshere")));
    }

    [TestMethod]
    public void SpreadsheetGetCellContents_Formula_ReturnsItself()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.IsTrue(new List<String> { "A1" }.SequenceEqual(ss.SetContentsOfCell("A1", "=56+4")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_NumberThenNonNumberSet_DoesntThrow()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "5.0");
        ss.SetContentsOfCell("A1", "five");
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_StringThenNonStringSet_DoesntThrow()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "five");
        ss.SetContentsOfCell("A1", "5.0");
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_FormulaThenNonFormulaSet_DoesntThrow()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=56+4");
        ss.SetContentsOfCell("A1", "five");
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_OneOrderOfPrecedence_ReturnsTwoThings()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("b1", "=a1 + 5");
        Assert.IsTrue(new List<String> { "A1", "B1" }.SequenceEqual(ss.SetContentsOfCell("A1", "=56+4")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_TwoOrderOfPrecedence_ReturnsThreeThings()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("c1", "=b1");
        ss.SetContentsOfCell("b1", "=a1");
        Assert.IsTrue(
            new List<String> { "A1", "B1", "C1" }.SequenceEqual(ss.SetContentsOfCell("A1", "=56+4")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_RespectsOrder_ReturnsExpectedResult()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("c1", "=b2");
        ss.SetContentsOfCell("c2", "=b2");
        ss.SetContentsOfCell("b2", "=a1");
        ss.SetContentsOfCell("b1", "=a1");
        Assert.IsTrue(
            new List<String> { "A1", "B1", "B2", "C2", "C1" }.SequenceEqual(ss.SetContentsOfCell("A1", "12")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_DependentChanged_ReturnsExpectedResult()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5.0");
        ss.SetContentsOfCell("b1", "=a1");
        Assert.IsTrue(new List<String> { "B1" }.SequenceEqual(ss.SetContentsOfCell("B1", "12")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_DependentChangedLongChain_ReturnsExpectedResult()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("a1", "5.0");
        ss.SetContentsOfCell("a2", "=a1");
        ss.SetContentsOfCell("a3", "=a2");
        ss.SetContentsOfCell("a4", "=a3");
        Assert.IsTrue(new List<String> { "A3", "A4" }.SequenceEqual(ss.SetContentsOfCell("A3", "12")));
        Assert.IsTrue(new List<String> { "A1", "A2" }.SequenceEqual(ss.SetContentsOfCell("A1", "5.001")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_OneOrderOfPrecedenceReversed_ReturnsTwoThings()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=56+4");
        Assert.IsTrue(new List<String> { "B1" }.SequenceEqual(ss.SetContentsOfCell("b1", "=a1 + 5")));
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCell_TwoOrderOfPrecedenceReversed_ReturnsThreeThings()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=56+4");
        ss.SetContentsOfCell("b1", "=a1");
        Assert.IsTrue(new List<String> { "C1" }.SequenceEqual(ss.SetContentsOfCell("c1", "=b1")));
    }


    [TestMethod]
    public void SpreadsheetGetCellContents_Number_ReturnsNumber()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("b1", "5.0");
        Assert.AreEqual(5.0, ss.GetCellContents("b1"));
    }

    [TestMethod]
    public void SpreadsheetGetCellContents_String_ReturnsString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("b1", "textgoeshere");
        Assert.AreEqual("textgoeshere", ss.GetCellContents("b1"));
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SpreadsheetGetCellContents_SelfLoop_ThrowsException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=A1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SpreadsheetGetCellContents_DirectCircularDependencies_ThrowsException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1");
        ss.SetContentsOfCell("B1", "=A1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SpreadsheetGetCellContents_IndirectCircularDependencies_ThrowsException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1");
        ss.SetContentsOfCell("B1", "=C1");
        ss.SetContentsOfCell("C1", "=D1");
        ss.SetContentsOfCell("D1", "=A1");
    }

    [TestMethod]
    public void SpreadsheetGetNamesOfAllNonEmptyCells_CellSetToEmptyString_ReturnsExpectedResult()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "12");
        ss.SetContentsOfCell("B1", "13");
        ss.SetContentsOfCell("A1", "");
        ss.SetContentsOfCell("C1", "14");
        ss.SetContentsOfCell("C1", string.Empty);
        Assert.IsTrue(new HashSet<string> { "B1" }.SequenceEqual(ss.GetNamesOfAllNonemptyCells()));
    }

    [TestMethod]
    public void SpreadsheetGetCellContents_InvalidCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual(ss.GetCellContents("A1"), string.Empty);
    }

    [TestMethod]
    public void SpreadsheetGetCellContents_AddedThenRemovedCell_ReturnsEmptyString()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=12");
        ss.SetContentsOfCell("A1", string.Empty);
        Assert.AreEqual(ss.GetCellContents("A1"), string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SpreadsheetSetContentsOfCell_InvalidNameOnDouble_Throws()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("ggh", "12.0");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SpreadsheetSetContentsOfCell_InvalidNameOnString_Throws()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("ggh", "hi");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SpreadsheetSetContentsOfCell_InvalidNameOnFormula_Throws()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("ggh", "=12 + 6");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SpreadsheetSetContentsOfCell_MultipleVarsWithOneCircularDependency_ThrowsException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1+A2");
        ss.SetContentsOfCell("C1", "=A1");
        ss.SetContentsOfCell("C2", "=C1");
        ss.SetContentsOfCell("A2", "=C2");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SpreadsheetGetCellContents_InvalidCellName_ThrowsException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1+A2");
        ss.GetCellContents("ghhh");
    }

    [TestMethod]
    public void SpreadsheetSetContentsOfCellFormula_CircularDependencyChangesNothing_ThrowsException()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1");
        ss.SetContentsOfCell("B1", "=C1");
        ss.SetContentsOfCell("C1", "=D1");
        try
        {
            ss.SetContentsOfCell("D1", "=A1");
        }
        catch (CircularException)
        {
            Assert.IsTrue(
                new HashSet<string> { "D1", "C1", "B1", "A1" }.SequenceEqual(ss.SetContentsOfCell("D1", "15")));
        }
    }

    // EMPTY SPREADSHEETS
    [TestMethod(), Timeout(10000)]
    [TestCategory("2")]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_InvalidName_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.GetCellContents("1AA");
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("3")]
    public void GetCellContents_EmptyCell_Works()
    {
        Spreadsheet s = new Spreadsheet();
        Assert.AreEqual("", s.GetCellContents("A2"));
    }

    // SETTING CELL TO A DOUBLE
    [TestMethod(), Timeout(10000)]
    [TestCategory("5")]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_InvalidNameDouble_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A1A", "1.5");
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("6")]
    public void SetAndGet_Double_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("Z7", "1.5");
        Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
    }

    // SETTING CELL TO A STRING
    [TestMethod(), Timeout(10000)]
    [TestCategory("9")]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_InvalidNameString_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1AZ", "hello");
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("10")]
    public void SetAndGet_String_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("Z7", "hello");
        Assert.AreEqual("hello", s.GetCellContents("Z7"));
    }

    // SETTING CELL TO A FORMULA
    [TestMethod(), Timeout(10000)]
    [TestCategory("13")]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_InvalidNameFormula_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1AZ", "=2");
    }

    // CIRCULAR FORMULA DETECTION
    [TestMethod(), Timeout(10000)]
    [TestCategory("15")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_Circular_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A2");
        s.SetContentsOfCell("A2", "=A1");
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("16")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_IndirectCircular_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A3", "=A4+A5");
        s.SetContentsOfCell("A5", "=A6+A7");
        s.SetContentsOfCell("A7", "=A1+A1");
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("17")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_Circular_UndoesCellChanges()
    {
        Spreadsheet s = new Spreadsheet();
        try
        {
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "15");
            s.SetContentsOfCell("A3", "30");
            s.SetContentsOfCell("A2", "=A3*A1");
        }
        catch (CircularException)
        {
            Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
            throw; // C# shortcut to rethrow the same exception that was caught
        }
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("17b")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_Circular_UndoesGraphChanges()
    {
        Spreadsheet s = new Spreadsheet();
        try
        {
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }
        catch (CircularException)
        {
            Assert.AreEqual("", s.GetCellContents("A2"));
            Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
            throw; // C# shortcut to rethrow the same exception that was caught
        }
    }

    // NONEMPTY CELLS

    [TestMethod(), Timeout(10000)]
    [TestCategory("20")]
    public void GetNames_NonemptyCellString_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "hello");
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("21")]
    public void GetNames_NonemptyCellDouble_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "52.25");
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("22")]
    public void GetNames_NonemptyCellFormula_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "=3.5");
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("23")]
    public void GetNames_NonemptyCells_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "17.2");
        s.SetContentsOfCell("C1", "hello");
        s.SetContentsOfCell("B1", "=3.5");
        Assert.IsTrue(
            new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
    }

    // RETURN VALUE OF SET CELL CONTENTS
    [TestMethod(), Timeout(10000)]
    [TestCategory("24")]
    public void SetContentsOfCell_Double_NoFalseDependencies()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "hello");
        s.SetContentsOfCell("C1", "=5");
        Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SequenceEqual(new List<string>() { "A1" }));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("25")]
    public void SetContentsOfCell_String_NoFalseDependencies()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "17.2");
        s.SetContentsOfCell("C1", "=5");
        Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("26")]
    public void SetContentsOfCell_Formula_NoFalseDependencies()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "17.2");
        s.SetContentsOfCell("B1", "hello");
        Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SequenceEqual(new List<string>() { "C1" }));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("27")]
    public void SetContentsOfCell_ChainDependencies_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A2", "6");
        s.SetContentsOfCell("A3", "=A2+A4");
        s.SetContentsOfCell("A4", "=A2+A5");
        Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SequenceEqual(new List<string>() { "A5", "A4", "A3", "A1" }));
    }

    // CHANGING CELLS
    [TestMethod(), Timeout(10000)]
    [TestCategory("28")]
    public void SetContentsOfCell_FormulaToDouble_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A1", "2.5");
        Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("29")]
    public void SetContentsOfCell_FormulaToString_Works()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A1", "Hello");
        Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
    }

    // STRESS TESTS
    [TestMethod(), Timeout(10000)]
    [TestCategory("31")]
    public void TestStress1()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=B1+B2");
        s.SetContentsOfCell("B1", "=C1-C2");
        s.SetContentsOfCell("B2", "=C3*C4");
        s.SetContentsOfCell("C1", "=D1*D2");
        s.SetContentsOfCell("C2", "=D3*D4");
        s.SetContentsOfCell("C3", "=D5*D6");
        s.SetContentsOfCell("C4", "=D7*D8");
        s.SetContentsOfCell("D1", "=E1");
        s.SetContentsOfCell("D2", "=E1");
        s.SetContentsOfCell("D3", "=E1");
        s.SetContentsOfCell("D4", "=E1");
        s.SetContentsOfCell("D5", "=E1");
        s.SetContentsOfCell("D6", "=E1");
        s.SetContentsOfCell("D7", "=E1");
        s.SetContentsOfCell("D8", "=E1");
        s.Save("sheet.json");
        IList<String> cells = s.SetContentsOfCell("E1", "0");
        Assert.IsTrue(new HashSet<string>()
                { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }
            .SetEquals(cells));
    }

    // Repeated for extra weight
    [TestMethod(), Timeout(10000)]
    [TestCategory("32")]
    public void TestStress1a()
    {
        TestStress1();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("33")]
    public void TestStress1b()
    {
        TestStress1();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("34")]
    public void TestStress1c()
    {
        TestStress1();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("35")]
    public void TestStress2()
    {
        Spreadsheet s = new Spreadsheet();
        ISet<String> cells = new HashSet<string>();
        for (int i = 1; i < 200; i++)
        {
            cells.Add("A" + i);
            Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
        }
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("36")]
    public void TestStress2a()
    {
        TestStress2();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("37")]
    public void TestStress2b()
    {
        TestStress2();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("38")]
    public void TestStress2c()
    {
        TestStress2();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("39")]
    public void TestStress3()
    {
        Spreadsheet s = new Spreadsheet();
        for (int i = 1; i < 200; i++)
        {
            s.SetContentsOfCell("A" + i, "=A" + (i + 1));
        }

        try
        {
            s.SetContentsOfCell("A150", "=A50");
            Assert.Fail();
        }
        catch (CircularException)
        {
        }
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("40")]
    public void TestStress3a()
    {
        TestStress3();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("41")]
    public void TestStress3b()
    {
        TestStress3();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("42")]
    public void TestStress3c()
    {
        TestStress3();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("43")]
    public void TestStress4()
    {
        Spreadsheet s = new Spreadsheet();
        for (int i = 0; i < 500; i++)
        {
            s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
        }

        LinkedList<string> firstCells = new LinkedList<string>();
        LinkedList<string> lastCells = new LinkedList<string>();
        for (int i = 0; i < 250; i++)
        {
            firstCells.AddFirst("A1" + i);
            lastCells.AddFirst("A1" + (i + 250));
        }

        Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
        Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("44")]
    public void TestStress4a()
    {
        TestStress4();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("45")]
    public void TestStress4b()
    {
        TestStress4();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("46")]
    public void TestStress4c()
    {
        TestStress4();
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("47")]
    public void TestStress5()
    {
        RunRandomizedTest(47, 2519);
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("48")]
    public void TestStress6()
    {
        RunRandomizedTest(48, 2521);
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("49")]
    public void TestStress7()
    {
        RunRandomizedTest(49, 2526);
    }

    [TestMethod(), Timeout(10000)]
    [TestCategory("50")]
    public void TestStress8()
    {
        RunRandomizedTest(50, 2521);
    }

    // Start of tests written for PS6

    [TestMethod]
    public void Changed_NewSpreadsheet_False()
    {
        Spreadsheet s = new Spreadsheet();
        Assert.IsFalse(s.Changed);
    }

    [TestMethod]
    public void Changed_ModifiedSpreadsheet_True()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void Changed_JustSavedSpreadsheet_False()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.Save("blank.txt");
        Assert.IsFalse(s.Changed);
    }

    [TestMethod]
    public void Changed_EditedSavedSpreadsheet_False()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.Save("blank.txt");
        s.SetContentsOfCell("A2", "7");
        Assert.IsTrue(s.Changed);
    }

    [TestMethod]
    public void GetCellValue_InputNumber_ReturnsCorrectValue()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.AreEqual(5.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_InputString_ReturnsCorrectValue()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "h");
        Assert.AreEqual("h", s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_InputFormula_ReturnsCorrectValue()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=5+2");
        Assert.AreEqual(7.0, s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_InputIncorrectFormula_ReturnsCorrectValue()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=5/0");
        Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("A1"));
    }

    [TestMethod]
    public void GetCellValue_TestIndexer_ReturnsCorrectValue()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.AreEqual(5.0, s["A1"]);
    }

    [TestMethod]
    public void GetCellValue_SetCellContents_CorrectlyUpdatesEverything()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A2", "5");
        s.SetContentsOfCell("A1", "=A2");
        Assert.AreEqual(5.0, s["A1"]);
        s.SetContentsOfCell("A2", "7");
        Assert.AreEqual(7.0, s["A1"]);
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveSpreadsheet_InvalidFilePath_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.Save("/missing/save.json");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadSpreadsheet_InvalidFilePath_Throws()
    {
        Spreadsheet _ = new Spreadsheet("/missing/save.json");
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadSpreadsheet_InvalidFileData_Throws()
    {
        File.WriteAllText("blank.json", string.Empty);
        try
        {
            Spreadsheet _ = new Spreadsheet("blank.json");
        }
        finally
        {
            File.Delete("blank.json");
        }
    }

    [TestMethod]
    public void SaveSpreadsheet_SavesToFile_EverythingWorks()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.Save("sheet.json");
        Assert.IsTrue(File.Exists("sheet.json"));
    }

    [TestMethod]
    public void LoadSpreadsheet_LoadsFromFile_RemembersValues()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.Save("sheet.json");
        Spreadsheet s2 = new Spreadsheet("sheet.json");
        Assert.AreEqual(5.0, s2.GetCellValue("A1"));
    }

    [TestMethod]
    public void LoadSpreadsheet_LoadsFromFile_RemembersDependencies()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=A2");
        s.SetContentsOfCell("A2", "5");
        s.Save("sheet.json");
        Spreadsheet s2 = new Spreadsheet("sheet.json");
        Assert.AreEqual(5.0, s2.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void LoadSpreadsheet_BlankFile_EverythingWorks()
    {
        Spreadsheet s = new Spreadsheet();
        s.Save("sheet.json");
        Spreadsheet s2 = new Spreadsheet("sheet.json");
        Assert.IsTrue(new HashSet<string>(s2.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>()));
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void SetContentsOfCell_InvalidFormula_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=++");
    }

    [TestMethod]
    public void GetCellValue_UsedIndexer_ReturnsCorrectValue()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        Assert.AreEqual(5.0, s["A1"]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellValue_UsedIndexer_Throws()
    {
        Spreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        _ = s["15"];
    }

    /// <summary>
    /// Sets random contents for a random cell 10000 times
    /// </summary>
    /// <param name="seed">Random seed</param>
    /// <param name="size">The known resulting spreadsheet size, given the seed</param>
    private void RunRandomizedTest(int seed, int size)
    {
        Spreadsheet s = new Spreadsheet();
        Random rand = new Random(seed);
        for (int i = 0; i < 10000; i++)
        {
            try
            {
                switch (rand.Next(3))
                {
                    case 0:
                        s.SetContentsOfCell(randomName(rand), "3.14");
                        break;
                    case 1:
                        s.SetContentsOfCell(randomName(rand), "hello");
                        break;
                    case 2:
                        s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                        break;
                }
            }
            catch (CircularException)
            {
            }
        }

        ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
        Assert.AreEqual(size, set.Count);
    }

    /// <summary>
    /// Generates a random cell name with a capital letter and number between 1 - 99
    /// </summary>
    /// <param name="rand"></param>
    /// <returns></returns>
    private string randomName(Random rand)
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
    }

    /// <summary>
    /// Generates a random Formula
    /// </summary>
    /// <param name="rand"></param>
    /// <returns></returns>
    private string randomFormula(Random rand)
    {
        string f = randomName(rand);
        for (int i = 0; i < 10; i++)
        {
            switch (rand.Next(4))
            {
                case 0:
                    f += "+";
                    break;
                case 1:
                    f += "-";
                    break;
                case 2:
                    f += "*";
                    break;
                case 3:
                    f += "/";
                    break;
            }

            switch (rand.Next(2))
            {
                case 0:
                    f += 7.2;
                    break;
                case 1:
                    f += randomName(rand);
                    break;
            }
        }

        return f;
    }
}