// <authors> bananathrowingmachine </authors>
// <date> September 3rd, 2024 </date>

using Spreadsheet.Formula;

namespace FormulaTests;

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
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///     <para>
    ///         This test makes sure the right kind of exception is thrown
    ///         when trying to create a formula with no tokens.
    ///     </para>
    ///     <remarks>
    ///         <list type="bullet">
    ///             <item>
    ///                 We use the _ (discard) notation because the formula object
    ///                 is not used after that point in the method.  Note: you can also
    ///                 use _ when a method must match an interface but does not use
    ///                 some of the required arguments to that method.
    ///             </item>
    ///             <item>
    ///                 string.Empty is often considered best practice (rather than using "") because it
    ///                 is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///             </item>
    ///             <item>
    ///                 The name of a test method should follow the MS standard:
    ///                 https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///             </item>
    ///             <item>
    ///                 All methods should be documented, but perhaps not to the same extent
    ///                 as this one.  The remarks here are for your educational
    ///                 purposes (i.e., a developer would assume another developer would know these
    ///                 items) and would be superfluous in your code.
    ///             </item>
    ///             <item>
    ///                 Notice the use of the attribute tag [ExpectedException] which tells the test
    ///                 that the code should throw an exception, and if it doesn't an error has occurred;
    ///                 i.e., the correct implementation of the constructor should result
    ///                 in this exception being thrown based on the given poorly formed formula.
    ///             </item>
    ///         </list>
    ///     </remarks>
    ///     <example>
    ///         <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///     </example>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);
    }

    [TestMethod]
    public void FormulaConstructor_TestNoTokens_Valid()
    {
        _ = new Formula("1");
    }

    // --- Tests for Valid Token Rule
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensAlphabet_Invalid()
    {
        _ = new Formula("a");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensNonAlphabet_Invalid()
    {
        _ = new Formula("#");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensGrid_Invalid()
    {
        _ = new Formula("1a");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensNegativeNumber_Invalid()
    {
        _ = new Formula("-5");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensNumbers_Valid()
    {
        _ = new Formula("1234567");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensBigScientificNumbers_Valid()
    {
        _ = new Formula("3E7");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensSmallScientificNumbers_Valid()
    {
        _ = new Formula("3E-7");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensNonIntegerNumbers_Valid()
    {
        _ = new Formula("5.782");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensSymbols_Valid()
    {
        _ = new Formula("5 / 3 * 2 + 1 - 4");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensParenthesis_Valid()
    {
        _ = new Formula("(5)");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensGrid_Valid()
    {
        _ = new Formula("a1");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokensBiggerGrid_Valid()
    {
        _ = new Formula("abc123");
    }


    [TestMethod]
    public void FormulaConstructor_TestValidLowercaseScientificNumbers_Valid()
    {
        _ = new Formula("5e7");
    }


    // --- Tests for Closing Parenthesis Rule
    [TestMethod]
    public void FormulaConstructor_NoParenthesis_Valid()
    {
        _ = new Formula("1");
    }

    [TestMethod]
    public void FormulaConstructor_OneLayerParenthesis_Valid()
    {
        _ = new Formula("(1)");
    }

    [TestMethod]
    public void FormulaConstructor_TwoLayerParenthesis_Valid()
    {
        _ = new Formula("((1))");
    }

    [TestMethod]
    public void FormulaConstructor_TwoSeparateParenthesisSets_Valid()
    {
        _ = new Formula("(1) + (3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_OneLayerParenthesis_Invalid()
    {
        _ = new Formula(")(1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TwoLayerParenthesis_Invalid()
    {
        _ = new Formula("()1)(");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TwoSeparateParenthesisSets_Invalid()
    {
        _ = new Formula("1) + (3)");
    }

    // --- Tests for Balanced Parentheses Rule

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_OneLayerParenthesisLeftOnly_Invalid()
    {
        _ = new Formula("(1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_OneLayerParenthesisRightOnly_Invalid()
    {
        _ = new Formula("1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TwoLayerParenthesisWithLeftOnlyLayer_Invalid()
    {
        _ = new Formula("((1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TwoLayerParenthesisWithRightOnlyLayer_Invalid()
    {
        _ = new Formula("(1))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TwoSeparateParenthesisSetsInteriorMissing_Invalid()
    {
        _ = new Formula("(1) + 3)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TwoSeparateParenthesisSetsExteriorMissing_Invalid()
    {
        _ = new Formula("1) + (3)");
    }

    // --- Tests for First Token Rule

    /// <summary>
    ///     <para>
    ///         Make sure a simple well-formed formula is accepted by the constructor (the constructor
    ///         should not throw an exception).
    ///     </para>
    ///     <remarks>
    ///         This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///         In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///     </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenGrid_Valid()
    {
        _ = new Formula("a1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenParenthesis_Valid()
    {
        _ = new Formula("(1 + 2)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstOperatorToken_Invalid()
    {
        _ = new Formula("/ 1");
    }

    // --- Tests for  Last Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenGrid_Valid()
    {
        _ = new Formula("1+a1");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenParenthesis_Valid()
    {
        _ = new Formula("(1 + 2)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastOperatorToken_Invalid()
    {
        _ = new Formula("1 +");
    }

    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestNumberAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("(1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestNumberAfterOperator_Valid()
    {
        _ = new Formula("1 / 5");
    }

    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("((1))");
    }

    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisAfterOperator_Valid()
    {
        _ = new Formula("1 / (5)");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableAfterOpeningParenthesis_Valid()
    {
        _ = new Formula("(a1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableAfterOperator_Valid()
    {
        _ = new Formula("1 / a1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisAfterOpeningParenthesis_Invalid()
    {
        _ = new Formula("()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisAfterOperator_Invalid()
    {
        _ = new Formula("(1 / )");
    }


    // --- Tests for Extra Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterNumbers_Valid()
    {
        _ = new Formula("5 / 2");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterNumbers_Valid()
    {
        _ = new Formula("(5 / 2)");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterVariable_Valid()
    {
        _ = new Formula("a1 / 2");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterVariable_Valid()
    {
        _ = new Formula("(a1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterClosingParenthesis_Valid()
    {
        _ = new Formula("(1) / 2");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterClosingParenthesis_Valid()
    {
        _ = new Formula("((1))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterNumber_Invalid()
    {
        _ = new Formula("5 ()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberAfterNumber_Invalid()
    {
        _ = new Formula("5 7");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterNumber_Invalid()
    {
        _ = new Formula("5 a1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterVariable_Invalid()
    {
        _ = new Formula("a1 ()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberAfterVariable_Invalid()
    {
        _ = new Formula("a1 7");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterVariable_Invalid()
    {
        _ = new Formula("a1 a1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(5)(6)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(1) 1");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(1) a1");
    }
    
    [TestMethod]
    public void FormulaConstructor_TestBiggerVariables_Valid()
    {
        _ = new Formula("abB245");
    }
    
    [TestMethod]
    public void FormulaConstructor_TestBiggerVariablesUnevenSides_Valid()
    {
        _ = new Formula("abb24");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBiggerVariablesSandwich_Invalid()
    {
        _ = new Formula("a23b");
    }
}