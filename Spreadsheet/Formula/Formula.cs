// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs [redacted], [redacted], and [redacted] as well as bananathrowingmachine.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// <date> September 14th, 2024 </date>
// </summary>

using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Spreadsheet.Formula;

/// <summary>
///     <para>
///         This class represents formulas written in standard infix notation using standard precedence
///         rules.  The allowed symbols are non-negative numbers written using double-precision
///         floating-point syntax; variables that consist of one or more letters followed by
///         one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///     </para>
///     <para>
///         Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///         a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///         and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///     </para>
/// </summary>
public class Formula
{
    /// <summary>
    ///     All variables are letters followed by numbers.  This pattern
    ///     represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";
    
    /// <summary>
    ///     This stores the string representation of the equation so it can be produced in O(1) time.
    /// </summary>
    private readonly string _equationString = string.Empty;
    
    /// <summary>
    ///     This stores all variables in the formula in a set, which is made during object construction.
    /// </summary>
    private readonly HashSet<string> _variables = [];
    
    /// <summary>
    ///     This stores all variables in the formula in a set, which is made during object construction.
    /// </summary>
    private readonly List<string> _tokens;
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="Formula" /> class.
    ///     <para>
    ///         Creates a Formula from a string that consists of an infix expression written as
    ///         described in the class comment.  If the expression is syntactically incorrect,
    ///         throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///         specifications for the syntax rules you are to implement.
    ///     </para>
    ///     <para>
    ///         Non-Exhaustive Example Errors:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///         </item>
    ///         <item>
    ///             Empty formula, e.g., string.Empty
    ///         </item>
    ///         <item>
    ///             Mismatched Parentheses, e.g., "(("
    ///         </item>
    ///         <item>
    ///             Invalid Following Rule, e.g., "2x+5"
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        _tokens = GetTokens(formula);
        if (_tokens.Count.Equals(0))
            throw new FormulaFormatException("This formula doesn't contain any tokens.");
        int leftParenthesisCount = 0;
        int rightParenthesisCount = 0;
        var tokenBefore = "start"; 
        foreach (string token in _tokens)
        {
            switch (token)
            {
                case "(":
                    leftParenthesisCount++;
                    break;
                case ")":
                {
                    rightParenthesisCount++;
                    if (rightParenthesisCount > leftParenthesisCount)
                        throw new FormulaFormatException(
                            "This formula has more right parenthesis than left parenthesis.");
                    break;
                }
                default:
                {
                    if (!(token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/") ||
                          IsVar(token) || double.TryParse(token, out _)))
                        throw new FormulaFormatException("This formula has an invalid token.");
                    break;
                }
            }
            VerifyFollowingRules(token, tokenBefore);
            tokenBefore = token;
            if (double.TryParse(token, out double number)) //setup for ToString
                _equationString += number;
            else
                _equationString += token;
        }
        if (leftParenthesisCount != rightParenthesisCount)
            throw new FormulaFormatException("This formula has an unequal amount of left vs right parenthesis.");
        if (!(_tokens[^1] == ")" || IsVar(_tokens[^1]) || double.TryParse(_tokens[^1], out _)))
            throw new FormulaFormatException("This formula has an invalid token in the back.");
    }

    /// <summary>
    ///     Makes sure that the token passed in has a correct token placed before it, as well as verifying if the first token is correct.
    /// </summary>
    /// <param name="token"> The token to verify.</param>
    /// <param name="tokenBefore"> The token that came before the current token.</param>
    private void VerifyFollowingRules(string token, string tokenBefore)
    {
        if (tokenBefore.Equals("start") && !(token == "(" || IsVar(token) || double.TryParse(token, out _))) 
            throw new FormulaFormatException("This formula has an invalid token in the front.");
        string tokenType = ClassifyToken(token);
        string tokenBeforeType = ClassifyToken(tokenBefore);
        switch (tokenType)
        {
            case "number":
            case "variable":
            case "(":
            {
                switch (tokenBeforeType)
                {
                    case "number":
                    case "variable":
                    case ")":
                    {
                        throw new FormulaFormatException("This formula has an invalid order of tokens.");
                    }
                }
                break;
            }
            case "smallOperator":
            case "bigOperator":
            case ")":
            {
                switch (tokenBeforeType)
                {
                    case "smallOperator":
                    case "bigOperator":
                    case "(":
                    {
                        throw new FormulaFormatException("This formula has an invalid order of tokens.");
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    ///     Will determine which type of token the input is (number, variable, etc.), as well as adding variables to the class HashMap of variables.
    /// </summary>
    /// <param name="token"> The token to classify. </param>
    /// <returns> The token type. </returns>
    private string ClassifyToken(string token)
    {
        if (double.TryParse(token, out _))
            return "number";
        if (!IsVar(token))
            return token switch
            {
                "+" or "-" => "smallOperator",
                "*" or "/" => "bigOperator",
                _ => token //will return the same parenthesis if given a parenthesis
            };
        _variables.Add(token); //adds the variables to the global set here, to avoid verifying if a token is a variable an excessive amount of times
        return "variable";
    }

    /// <summary>
    ///     <para>
    ///         Returns a set of all the variables in the formula.
    ///     </para>
    ///     <remarks>
    ///         Important: no variable may appear more than once in the returned set, even
    ///         if it is used more than once in the Formula.
    ///     </remarks>
    ///     <para>
    ///         For example, if N is a method that converts all the letters in a string to upper case:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///         <item>new("x1+X1"   ).GetVariables() should enumerate "X1".</item>
    ///     </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        return _variables;
    }

    /// <summary>
    ///     <para>
    ///         Returns a string representation of a canonical form of the formula.
    ///     </para>
    ///     <para>
    ///         The string will contain no spaces.
    ///     </para>
    ///     <para>
    ///         If the string is passed to the Formula constructor, the new Formula f
    ///         will be such that this.ToString() == f.ToString().
    ///     </para>
    ///     <para>
    ///         All the variables in the string will be normalized.  This
    ///         means capital letters.
    ///     </para>
    ///     <para>
    ///         For example:
    ///     </para>
    ///     <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///     <para>
    ///         This code should execute in O(1) time.
    ///     </para>
    /// </summary>
    /// <returns>
    ///     A canonical version (string) of the formula. All "equal" formulas
    ///     should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return _equationString;
    }

    /// <summary>
    ///     Reports whether "token" is a variable.  It must be one or more letters
    ///     followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        var standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///     <para>
    ///         Given an expression, enumerates the tokens that compose it.
    ///     </para>
    ///     <para>
    ///         Tokens returned are:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>left paren</item>
    ///         <item>right paren</item>
    ///         <item>one of the four operator symbols</item>
    ///         <item>a string consisting of one or more capital letters followed by one or more numbers</item>
    ///         <item>a double literal</item>
    ///         <item>and anything that doesn't match one of the above patterns</item>
    ///     </list>
    ///     <para>
    ///         There are no empty tokens; white space is ignored (except to separate other tokens).
    ///     </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        var lpPattern = @"\(";
        var rpPattern = @"\)";
        var opPattern = @"[\+\-*/]";
        var doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        var spacePattern = @"\s+";

        // Overall pattern
        var pattern = string.Format(
            "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
            lpPattern,
            rpPattern,
            opPattern,
            VariableRegExPattern,
            doublePattern,
            spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (var s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                results.Add(s.ToUpper());

        return results;
    }
    
    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference 
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.  
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals( object? obj )
    {
        return obj is Formula && ToString().Equals(obj.ToString());
    }
    
    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==( Formula f1, Formula f2 )
    {
        return f1.Equals(f2);
    }
    
    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=( Formula f1, Formula f2 )
    {
        return !f1.Equals(f2);
    }
    
    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode( )
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a 
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect 
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>

    public object Evaluate(Lookup lookup)
    {
        Stack<double> valueStack = new Stack<double>();
        Stack<string> operatorStack = new Stack<string>();
        foreach (string token in _tokens)
        {
            switch (ClassifyToken(token))
            {
                case "number":
                {
                    FormulaError? error = CheckForBigOperators(valueStack, operatorStack, double.Parse(token));
                    if (error != null)
                        return error;
                    break;
                }
                case "variable":
                {
                    try
                    {
                        FormulaError? error = CheckForBigOperators(valueStack, operatorStack, lookup(token));
                        if (error != null)
                            return error;
                    } 
                    catch (ArgumentException)
                    {
                        return new FormulaError($"Variable '{token}' is not defined.");
                    }
                    break;
                }
                case "smallOperator":
                {
                    CheckForSmallOperators(valueStack, operatorStack, valueStack.Pop());
                    goto case "(";
                }
                case "bigOperator":
                case "(":
                {
                    operatorStack.Push(token);
                    break;
                }
                case ")":
                {
                    CheckForSmallOperators(valueStack, operatorStack, valueStack.Pop());
                    operatorStack.Pop();
                    FormulaError? error = CheckForBigOperators(valueStack, operatorStack, valueStack.Pop());
                    if (error != null)
                        return error;
                    break;
                }
            }
        }
        CheckForSmallOperators(valueStack, operatorStack, valueStack.Pop());
        return valueStack.Pop();
    }
    
    /// <summary>
    ///     Will check for and then evaluate a big operator (*, /) which the stacks and numbers provided. If there is no small operator, number 2 will get pushed.
    /// </summary>
    /// <param name="valueStack">The stack of values.</param>
    /// <param name="operatorStack">The stack of operators.</param>
    /// <param name="number2">The number to the right of an operator.</param>
    /// <returns>A formula error if there was one.</returns>
    private FormulaError? CheckForBigOperators(Stack<double> valueStack, Stack<string> operatorStack, double number2)
    {
        if (operatorStack.Count != 0 && ClassifyToken(operatorStack.Peek()) == "bigOperator")
        {
            if (operatorStack.Pop() == "*")
                valueStack.Push(valueStack.Pop() * number2);
            else
            {
                if (number2 == 0)
                    return new FormulaError("Cannot divide by zero.");
                valueStack.Push(valueStack.Pop() / number2);
            }
            return null;
        }
        valueStack.Push(number2);
        return null;
    }
    
    /// <summary>
    ///     Will check for and then evaluate a small operator (+, -) which the stacks and numbers provided. If there is no small operator, number 2 will get pushed.
    /// </summary>
    /// <param name="valueStack">The stack of values.</param>
    /// <param name="operatorStack">The stack of operators.</param>
    /// <param name="number2">The number to the right of an operator.</param>
    private void CheckForSmallOperators(Stack<double> valueStack, Stack<string> operatorStack, double number2)
    {
        if (operatorStack.Count != 0 && ClassifyToken(operatorStack.Peek()) == "smallOperator")
        {
            if (operatorStack.Pop() == "+")
                valueStack.Push(valueStack.Pop() + number2);
            else
                valueStack.Push(valueStack.Pop() - number2);
            return;
        }
        valueStack.Push(number2);
    }
}

/// <summary>
///     Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FormulaFormatException" /> class.
    ///     <para>
    ///         Constructs a FormulaFormatException containing the explanatory message.
    ///     </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}

public class FormulaError
{
    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError( string message )
    {
        Reason = message;
    }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup( string variableName );