// <authors> bananathrowingmachine </authors>
// <date> September 14rd, 2024 </date>

using Spreadsheet.Formula;

namespace FormulaTests;

[TestClass]
public class FormulaEvaluationTests
{
    [TestMethod]
    public void FormulaEquals_Spaced_True()
    {
        Assert.IsTrue(new Formula("A1+3-(12*7)").Equals(new Formula("A1 + 3 - ( 12 * 7 )")));
    }
    
    [TestMethod]
    public void FormulaEquals_NotAEquation_False()
    {
        Assert.IsFalse(new Formula("A1+3-(12*7)").Equals(null));
    }
    
    [TestMethod]
    public void FormulaOperatorEquals_Spaced_True()
    {
        Assert.IsTrue(new Formula("A1+3-(12*7)") == new Formula("A1 + 3 - ( 12 * 7 )"));
    }
    
    [TestMethod]
    public void FormulaOperatorNotEquals_DifferentFormulas_True()
    {
        Assert.IsTrue(new Formula("A1+3-(12*7)") != new Formula("A1+4-(12*7)"));
    }
    
    [TestMethod]
    public void FormulaEquals_NotEqualFormulas_False()
    {
        Assert.IsFalse(new Formula("A1+3-(12*7)").Equals(new Formula("A1+4-(12*7)")));
    }
    
    [TestMethod]
    public void FormulaOperatorEquals_NotEqualFormulas_False()
    {
        Assert.IsFalse(new Formula("A1+3-(12*7)") == new Formula("A1+4-(12*7)"));
    }
    
    [TestMethod]
    public void FormulaOperatorNotEquals_SameFormulas_False()
    {
        Assert.IsFalse(new Formula("A1+3-(12*7)") != new Formula("a1 + 3-(12*7)"));
    }
    
    [TestMethod]
    public void FormulaEquals_Caps_True()
    {
        Assert.IsTrue(new Formula("a1+3-(12*7)").Equals(new Formula("A1+3-(12*7)")));
    }
    
    [TestMethod]
    public void FormulaOperatorEquals_Caps_True()
    {
        Assert.IsTrue(new Formula("a1+3-(12*7)") == new Formula("A1+3-(12*7)"));
    }
    
    [TestMethod]
    public void FormulaGetHashCode_Spaced_AreEqual()
    {
        Assert.AreEqual(new Formula("A1+3-(12*7)").GetHashCode(), new Formula("A1 + 3 - ( 12 * 7 )").GetHashCode());
    }
    
    [TestMethod]
    public void FormulaGetHashCode_Caps_AreEqual()
    {
        Assert.AreEqual(new Formula("A1+3-(12*7)").GetHashCode(), new Formula("a1 + 3-(12*7)").GetHashCode());
    }
    
    [TestMethod]
    public void FormulaGetHashCode_DifferentFormulas_AreNotEqual()
    {
        Assert.AreNotEqual(new Formula("A1+3-(12*7)").GetHashCode(), new Formula("12 + a3 - A4").GetHashCode());
    }

    [TestMethod]
    public void FormulaEvaluate_LookupIsSet_IsCorrect()
    {
        Assert.AreEqual(2.0, new Formula("a1").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_LookupIsVariable_IsCorrect()
    {
        Assert.AreEqual(4.74, new Formula("F2+P2+W2").Evaluate(Lookup1));
    }
    private double Lookup1(string var)
    {
        char[] charArray = var.ToCharArray();
        return charArray[0] / (double) charArray[1];
    }
    
    [TestMethod]
    public void FormulaEvaluate_LookupFails_IsInvalid()
    {
        Assert.IsInstanceOfType<FormulaError>(new Formula("a2+b2").Evaluate(Lookup2));
    }
    private double Lookup2(string var)
    {
        if (var == "A2")
            return 1.0;
        throw new ArgumentException();
    }
    
    [TestMethod]
    public void FormulaEvaluate_DivideByZero_IsInvalid()
    {
        Assert.IsInstanceOfType<FormulaError>(new Formula("1/0").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_BasicExpression_ReturnsCorrectResult()
    {
        Assert.AreEqual(8.0, new Formula("5 + 3").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_OrderOfOperations_ReturnsCorrectResult()
    {
        Assert.AreEqual(7.0, new Formula("5 + 4 / 2").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_Parenthesis_ReturnsCorrectResult()
    {
        Assert.AreEqual(4.5, new Formula("(5 + 4) / 2").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_MultiLayeredParenthesis_ReturnsCorrectResult()
    {
        Assert.AreEqual(55.0, new Formula("5 * (2 + 3 * (5-2))").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_ParenthesisBecomeZero_IsInvalid()
    {
        Assert.IsInstanceOfType<FormulaError>(new Formula("1/(5-5)").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_MultiArgumentParenthesis_ReturnsCorrectResult()
    {
        Assert.AreEqual(32.0, new Formula("2 * (1 + 3 * 5)").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_HandlesCapsVariablesEqually_ReturnsCorrectResult()
    {
        Assert.AreEqual(15.0, new Formula("A2*(a2-b3)").Evaluate(Lookup3));
    }
    private double Lookup3(string var)
    {
        if (var == "A2")
            return 5.0;
        return 2.0;
    }
    
    [TestMethod]
    public void FormulaEvaluate_HandlesHugeExpression_ReturnsCorrectResult()
    {
        Assert.AreEqual(9.0, new Formula("5-6/b5+(5+2*(3-6+a1)/(1-2))").Evaluate(var => var.Length));
    }
    
    [TestMethod]
    public void FormulaEvaluate_VariableDivideByZero_ReturnsCorrectResult()
    {
        Assert.IsInstanceOfType<FormulaError>(new Formula("5/a3").Evaluate(var => var.Length - 2));
    }
    
    [TestMethod]
    public void FormulaError_ReasonIsString_ReturnsCorrectResult()
    {
        Assert.IsInstanceOfType<String>(((FormulaError) new Formula("5/a3").Evaluate(var => var.Length - 2)).Reason);
    }
    
    [TestMethod]
    public void FormulaEvaluate_0divVar_Returns0()
    {
        Assert.AreEqual(0.0, new Formula("0/5").Evaluate(var => var.Length));
    }
}