// <authors> bananathrowingmachine </authors>
// <date> September 3rd, 2024 </date>

using Spreadsheet.Formula;

namespace FormulaTests;

[TestClass]
public class FormulaExtraMethodsTests
{
    [TestMethod]
    public void FormulaToString_BasicTest_ShouldReturnExpectedResult()
    {
        Assert.AreEqual("A1+3-(12*7)", new Formula("A1+3-(12*7)").ToString());
    }
    
    [TestMethod]
    public void FormulaToString_Spaced_ShouldReturnExpectedResult()
    {
        Assert.AreEqual("A1+3-(12*7)", new Formula("A1 + 3 - ( 12 * 7 )").ToString());
    }
    
    [TestMethod]
    public void FormulaToString_LowercaseVariables_ShouldReturnExpectedResult()
    {
        Assert.AreEqual("A1+3-(12*7)", new Formula("a1+3-(12*7)").ToString());
    }
    
    [TestMethod]
    public void FormulaToString_BothComputationallyEqual_ShouldReturnExpectedResult()
    {
        Assert.AreEqual(new Formula("((a1+2)*7)+B5").ToString(), new Formula("((A1 +2)* 7) + b5").ToString());
    }
    
    [TestMethod]
    public void FormulaToString_SpecialNumbers_ShouldReturnExpectedResult()
    {
        Assert.AreEqual("0.05+700-5", new Formula("5e-2+7E2-5.00000").ToString());
    }
    
    [TestMethod]
    public void FormulaGetVariables_BasicTest_ShouldReturnExpectedResult()
    {
        ISet<string> variables = new HashSet<string>();
        variables.Add("A1");
        variables.Add("B5");
        Assert.IsTrue(variables.SetEquals(new Formula("((A1+2)*7)+B5").GetVariables()));
    }
    
    [TestMethod]
    public void FormulaGetVariables_DuplicatedVars_ShouldReturnExpectedResult()
    {
        ISet<string> variables = new HashSet<string>();
        variables.Add("A1");
        variables.Add("B5");
        Assert.IsTrue(variables.SetEquals(new Formula("A1+A1-A1+B5*B5+B5/A1").GetVariables()));
    }
    
    [TestMethod]
    public void FormulaGetVariables_LowercaseInputs_ShouldReturnExpectedResult()
    {
        ISet<string> variables = new HashSet<string>();
        variables.Add("A1");
        variables.Add("B5");
        Assert.IsTrue(variables.SetEquals(new Formula("a1-b5").GetVariables()));
    }
    
    [TestMethod]
    public void FormulaGetVariables_BiggerVariables_ShouldReturnExpectedResult()
    {
        ISet<string> variables = new HashSet<string>();
        variables.Add("BBB1");
        variables.Add("A3");
        Assert.IsTrue(variables.SetEquals(new Formula("bbb1 + a3").GetVariables()));
    }
    
    [TestMethod]
    public void FormulaGetVariables_BothComputationallyEqual_ShouldReturnExpectedResult()
    {
        Assert.IsTrue(new Formula("((a1+2)*7)+B5").GetVariables().SetEquals(new Formula("((a1+2)*7)+B5").GetVariables()));
    }
    
    [TestMethod]
    public void FormulaGetVariables_LowercaseInputsAndDuplicates_ShouldReturnExpectedResult()
    {
        ISet<string> variables = new HashSet<string>();
        variables.Add("A1");
        variables.Add("B5");
        Assert.IsTrue(variables.SetEquals(new Formula("A1-b5+B5/a1-a1*B5").GetVariables()));
    }
}