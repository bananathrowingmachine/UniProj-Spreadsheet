// <summary>
//   <para>
//     This code was written by bananathrowingmachine, dated Sep 12, 2024
//   </para>
//   <para>

// ReSharper disable once CheckNamespace
namespace DevelopmentTests;

using Spreadsheet.DependencyGraph;

/// <summary>
/// This is a test class for DependencyGraphTest and is intended
/// to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// This test makes sure everything is working in high variable situations by preparing a bunch of variables, getting the correct answers, and then making and removing dependency pairs twice. 
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();
// A bunch of strings to use
        const int size = 200;
        string[] letters = new string[size];
        for (int i = 0; i < size; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

// The correct answers
        HashSet<string>[] dependents = new HashSet<string>[size];
        HashSet<string>[] dependees = new HashSet<string>[size];
        for (int i = 0; i < size; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

// Add a bunch of dependencies
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

// Remove a bunch of dependencies
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 4; j < size; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

// Add some back
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

// Remove some more
        for (int i = 0; i < size; i += 2)
        {
            for (int j = i + 3; j < size; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

// Make sure everything is right
        for (int i = 0; i < size; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new
                HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new
                HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }
    
    [TestMethod]
    public void Size_Initialized_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(0, dg.Size);
    }
    
    [TestMethod]
    public void Size_PairsAdded_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "b2");
        dg.AddDependency("b1", "a2");
        Assert.AreEqual(2, dg.Size);
    }
    
    [TestMethod]
    public void Size_PairsAddedThenRemoved_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "b2");
        dg.AddDependency("b1", "a2");
        dg.RemoveDependency("a1", "b2");
        Assert.AreEqual(1, dg.Size);
    }

    [TestMethod]
    public void HasDependents_NoDependents_False()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependents("a1"));
    }
    
    [TestMethod]
    public void HasDependents_HasDependents_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        Assert.IsTrue(dg.HasDependents("a2"));
    }
    
    [TestMethod]
    public void HasDependents_IsOnlyDependee_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        Assert.IsFalse(dg.HasDependents("a2"));
    }
    
    [TestMethod]
    public void HasDependents_VariableNeverAdded_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("b1", "a2");
        Assert.IsFalse(dg.HasDependents("a1"));
    }
    
    [TestMethod]
    public void HasDependees_NoDependees_False()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependents("a1"));
    }
    
    [TestMethod]
    public void HasDependees_HasDependees_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        Assert.IsTrue(dg.HasDependees("a1"));
    }
    
    [TestMethod]
    public void HasDependees_IsOnlyDependent_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        Assert.IsFalse(dg.HasDependees("a1"));
    }
    
    [TestMethod]
    public void HasDependees_VariableNeverAdded_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("b1", "a2");
        Assert.IsFalse(dg.HasDependees("a1"));
    }
    
    [TestMethod]
    public void GetDependents_Initialized_IsEmpty()
    {
        DependencyGraph dg = new();
        Assert.IsTrue(dg.GetDependents("a1").SequenceEqual(new HashSet<string>()));
    }
    
    [TestMethod]
    public void GetDependents_PairsAdded_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        Assert.IsTrue(dg.GetDependents("a1").SequenceEqual(new HashSet<string> {"a2", "a3"}));
    }
    
    [TestMethod]
    public void GetDependents_PairsAddedThenRemoved_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.RemoveDependency("a1", "a2");
        Assert.IsTrue(dg.GetDependents("a1").SequenceEqual(new HashSet<string> {"a3"}));
    }
    
    [TestMethod]
    public void GetDependees_Initialized_IsEmpty()
    {
        DependencyGraph dg = new();
        Assert.IsTrue(dg.GetDependees("a1").SequenceEqual(new HashSet<string>()));
    }
    
    [TestMethod]
    public void GetDependees_PairsAdded_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a3", "a1");
        Assert.IsTrue(dg.GetDependees("a1").SequenceEqual(new HashSet<string> {"a2", "a3"}));
    }
    
    [TestMethod]
    public void GetDependees_PairsAddedThenRemoved_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a3", "a1");
        dg.RemoveDependency("a2", "a1");
        Assert.IsTrue(dg.GetDependees("a1").SequenceEqual(new HashSet<string> {"a3"}));
    }
    
    [TestMethod]
    public void AddDependency_Duplicate_Valid() //most other things to test for add and remove dependency on are already tested above
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a2", "a3");
        dg.AddDependency("a2", "a1");
        Assert.IsTrue(dg.GetDependents("a2").SequenceEqual(new HashSet<string> {"a1", "a3"}));
    }

    [TestMethod]
    public void RemoveDependency_DependencyDoesntExist_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.RemoveDependency("a2", "a3");
        Assert.IsTrue(dg.GetDependents("a2").SequenceEqual(new HashSet<string> {"a1"}));
    }

    [TestMethod]
    public void RemoveDependency_Duplicate_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a2", "a3");
        dg.RemoveDependency("a2", "a1");
        dg.RemoveDependency("a2", "a1");
        Assert.IsTrue(dg.GetDependents("a2").SequenceEqual(new HashSet<string> {"a3"}));
    }

    [TestMethod]
    public void ReplaceDependents_EmptyList_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a2", "a3");
        dg.ReplaceDependents("a2", new HashSet<string>());
        Assert.IsTrue(dg.GetDependents("a2").SequenceEqual(new HashSet<string>()));
    }
    
    [TestMethod]
    public void ReplaceDependents_ListOfOne_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a2", "a3");
        dg.ReplaceDependents("a2", new HashSet<string>{"a4"});
        Assert.IsTrue(dg.GetDependents("a2").SequenceEqual(new HashSet<string>{"a4"}));
    }
    
    [TestMethod]
    public void ReplaceDependents_ListOfMany_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a2", "a1");
        dg.AddDependency("a2", "a3");
        dg.ReplaceDependents("a2", new HashSet<string>{"a4", "a5", "a6"});
        Assert.IsTrue(dg.GetDependents("a2").SequenceEqual(new HashSet<string>{"a4", "a5", "a6"}));
    }
    
    [TestMethod]
    public void ReplaceDependees_EmptyList_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a3", "a2");
        dg.ReplaceDependees("a2", new HashSet<string>());
        Assert.IsTrue(dg.GetDependees("a2").SequenceEqual(new HashSet<string>()));
    }
    
    [TestMethod]
    public void ReplaceDependees_ListOfOne_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a3", "a2");
        dg.ReplaceDependees("a2", new HashSet<string>{"a4"});
        Assert.IsTrue(dg.GetDependees("a2").SequenceEqual(new HashSet<string>{"a4"}));
    }
    
    [TestMethod]
    public void ReplaceDependees_ListOfMany_ReturnsExpectedResult()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a3", "a2");
        dg.ReplaceDependees("a2", new HashSet<string>{"a4", "a5", "a6"});
        Assert.IsTrue(dg.GetDependees("a2").SequenceEqual(new HashSet<string>{"a4", "a5", "a6"}));
    }
}