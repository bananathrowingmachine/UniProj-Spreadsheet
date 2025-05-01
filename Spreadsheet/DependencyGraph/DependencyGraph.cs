// Skeleton implementation written by [redacted] for CS 3500, September 2013
// Version 1.1 - [redacted]
//   (Fixed error in comment for RemoveDependency)
// Version 1.2 - [redacted] Fall 2018
//   (Clarified meaning of dependent and dependee)
//   (Clarified names in solution/project structure)
// Version 1.3 - [redacted] Fall 2024
// Version 1.4 - bananathrowingmachine, Sep 12, 2024

// ReSharper disable once CheckNamespace
namespace Spreadsheet.DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a")  = {}
///     dependees("b")  = {"a"}
///     dependees("c")  = {"a"}
///     dependees("d")  = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    /// Stores a list of dependents (cells that rely on the key) for each cell
    /// </summary>
    private Dictionary<string, HashSet<string>> _dependentsDict;
    /// <summary>
    /// Stores a list of dependees (cells that the key relies on) for each cell
    /// </summary>
    private Dictionary<string, HashSet<string>> _dependeesDict;

    /// <summary>
    /// Stores the amount of ordered pairs in the DependencyGraph
    /// </summary>
    public int Count
    {
        get;
        private set;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        _dependentsDict = new Dictionary<string, HashSet<string>>();
        _dependeesDict = new Dictionary<string, HashSet<string>>();
    }

    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size => Count;

    /// <summary>
    ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        return _dependentsDict.ContainsKey(nodeName);
    }

    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        return _dependeesDict.ContainsKey(nodeName);
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (!_dependentsDict.TryGetValue(nodeName, out var dependents))
            return new HashSet<string>();
        return dependents;
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if (!_dependeesDict.TryGetValue(nodeName, out var dependees))
            return new HashSet<string>();
        return dependees;
    }

    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    ///
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        bool increased = false;
        if (!_dependentsDict.ContainsKey(dependee))
        {
            _dependentsDict.Add(dependee, [dependent]);
            increased = true;
        }
        else
        {
            if (!_dependentsDict[dependee].Contains(dependent))
            {
                _dependentsDict[dependee].Add(dependent);
                increased = true;
            }
        }

        if (!_dependeesDict.ContainsKey(dependent))
        {
            _dependeesDict.Add(dependent, [dependee]);
            increased = true;
        }
        else 
            if (!_dependeesDict[dependent].Contains(dependee)) {
                _dependeesDict[dependent].Add(dependee);
                increased = true;
            }
        if(increased)
            Count++;
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists.
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        bool decreased = false;
        if (_dependentsDict.ContainsKey(dependee) && _dependentsDict[dependee].Contains(dependent))
        {
            _dependentsDict[dependee].Remove(dependent);
            decreased = true;
            if(_dependentsDict[dependee].Count == 0)
                _dependentsDict.Remove(dependee);
        }
        if (_dependeesDict.ContainsKey(dependent) && _dependeesDict[dependent].Contains(dependee))
        {
            _dependeesDict[dependent].Remove(dependee);
            decreased = true;
            if(_dependeesDict[dependent].Count == 0)
                _dependeesDict.Remove(dependent);
        }
        if (decreased)
            Count--;
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node whose dependents are being replaced </param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        if(_dependentsDict.TryGetValue(nodeName, out var value))
            foreach (string dependent in value)
            {
                RemoveDependency(nodeName, dependent);
            }
        foreach (string dependent in newDependents)
        {
            AddDependency(nodeName, dependent);
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        if(_dependeesDict.TryGetValue(nodeName, out var value))
            foreach (string dependee in value)
            {
              RemoveDependency(dependee, nodeName);
            }
        foreach (string dependee in newDependees)
        {
            AddDependency(dependee, nodeName);
        }
    }
}