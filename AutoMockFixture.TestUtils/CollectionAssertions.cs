using AutoMockFixture.Moq4;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AutoMockFixture.TestUtils;

internal static class CollectionAssertions
{

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllNotBeOfType<T, TExpected>(this GenericCollectionAssertions<T> assertions)
        => ExecuteInternal(assertions, item => item?.GetType() != typeof(TExpected) && (item is not Type t || t != typeof(TExpected)), "of type " + typeof(TExpected).Name);

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllNotBeAssignableTo <T, TExpected>(this GenericCollectionAssertions<T> assertions)
        => ExecuteInternal(assertions, item => item is not TExpected && (item is not Type t || !t.IsAssignableTo(typeof(TExpected))), "assignable to " + typeof(TExpected).Name);

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllBeSameAs<T>(this GenericCollectionAssertions<T> assertions, T t)
        => ExecuteInternal(assertions, item => ReferenceEquals(item, t), "different than provided");

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllNotBeSameAs<T>(this GenericCollectionAssertions<T> assertions, T t)
        => ExecuteInternal(assertions, item => !ReferenceEquals(item, t), "the same as provided");

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllBeNull<T>(this GenericCollectionAssertions<T> assertions)
        => ExecuteInternal(assertions, item => item is null, "non null", "not null");

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllBeNonNull<T>(this GenericCollectionAssertions<T> assertions)
        => ExecuteInternal(assertions, item => item is not null, "null");

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<bool>> AllBeTrue(this GenericCollectionAssertions<bool> assertions)
        => ExecuteInternal(assertions, item => item is true, "false");

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<bool>> AllBeFalse(this GenericCollectionAssertions<bool> assertions)
        => ExecuteInternal(assertions, item => item is false, "true");

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<T>> AllHaveNullState<T>(
            this GenericCollectionAssertions<T> assertions, bool isNull)
        => isNull ? assertions.AllBeNull() : assertions.AllBeNonNull();

    [CustomAssertion]
    internal static AndConstraint<GenericCollectionAssertions<T>> ExecuteInternal<T>(
            GenericCollectionAssertions<T> assertions, Func<T, bool> predicate,
            string msgPrefix1, string? msgPrefix2 = null)
    {
        var indexes = new List<int> { };
        var i = 0;
        foreach (var item in assertions.Subject)
        {
            if (!predicate(item)) indexes.Add(i);
            i++;
        }

        if (indexes.Count == assertions.Subject.Count()) { Execute.Assertion.FailWith($"All entries are {msgPrefix2 ?? msgPrefix1}"); }
        else if(indexes.Count > 0)
        {
            string message = $"Found {indexes.Count} {msgPrefix1}, at position\\s '{string.Join(", ",indexes)}'";
            var names = DetermineCallerIdentity();
            if(names is not null)
            {
                var masker = new ParenthesisMasker();
                var masked = masker.Mask(names);
                var nameList = masker.Unmask(masked.Split(",")).ToArray();
                var actual = new List<string> { };
                foreach (var item in indexes)
                {
                    if(item < nameList.Length) actual.Add(nameList[item]);
                }
                if(actual.Count > 0)
                {
                    message = $"The following {(actual.Count > 1 ? "are" : "is")} {msgPrefix2 ?? msgPrefix1}: "
                        + (actual.Count > 1 ? Environment.NewLine + "\t" : "")
                        + string.Join(Environment.NewLine + "\t" , actual.Select(s => s.Trim()));
                }
            }
            Execute.Assertion.FailWith(message);
        }

        return new AndConstraint<GenericCollectionAssertions<T>>((GenericCollectionAssertions<T>)assertions);
    }

    [CustomAssertion]
    private static string? DetermineCallerIdentity()
    {
        try
        {
            foreach (StackFrame stackFrame in new StackTrace(true).GetFrames())
            {
                if (stackFrame.GetMethod() is var method
                    && method?.GetCustomAttribute<CustomAssertionAttribute>(true) is null
                    && method!.DeclaringType is var declare && declare is not null
                    && declare.Namespace is var ns && ns?.StartsWith("System.") != true && ns != "System")
                {
                    if (stackFrame.GetFileName() is null || !File.Exists(stackFrame.GetFileName())) return null;

                    return GetData(stackFrame.GetFileName()!, stackFrame.GetFileLineNumber());
                }
            }
        }
        catch (Exception) {}
        return null;

        static string? GetData(string fileName, int fileLineNumber)
        {
            var lines = File.ReadAllLines(fileName);
            var text = lines.Skip(fileLineNumber - 1).FirstOrDefault();

            int index = 0;
            for (var toAdd = 1; text is null || (index = text.IndexOf("Should", StringComparison.InvariantCulture)) <= 0; toAdd++)
            {
                if (fileLineNumber + toAdd > lines.Length) return null;

                text += lines.Skip(fileLineNumber + toAdd).FirstOrDefault();
            };

            var subString = text.Substring(0, index - 1);
            var skipped = 0;
            while (skipped <= fileLineNumber && string.IsNullOrWhiteSpace(subString))
            {
                skipped++;
                subString = lines.Skip(fileLineNumber - skipped).FirstOrDefault();
            }

            char c = '\0';
            // A collection expression can end with ) if casting to the type
            if (subString?.TrimEnd().TrimEnd(')').EndsWith("]") == true) c = '[';
            else if (subString?.TrimEnd().TrimEnd(')').EndsWith("}") == true) c = '{';
            else return null;

            while ((index = subString.LastIndexOf(c)) < 0 && skipped < fileLineNumber)
            {
                skipped++;
                subString = lines.Skip(fileLineNumber - skipped).FirstOrDefault() + subString;
            }

            return subString.Substring(index + 1, subString.TrimEnd().Length - 2 - index);
        }
    }

    private abstract class MaskerBase
    {
        protected abstract Regex Regex { get; }
        protected Dictionary<int, string> replaceDict = new();

        protected abstract string ReplaceKey(int key);

        public string Mask(string str)
        {
            Match? match = null;

            // Remeber that generics can be nested so we start with the simplest ones and work our way from there

            for (var i = 0; (match = Regex.Match(str)).Success; i++)
            {
                str = str.Replace(match.Value, ReplaceKey(i));
                replaceDict[i] = match.Value;
            }

            return str;
        }

        public IEnumerable<string> Unmask(IEnumerable<string> strs)
        {
            // Remeber that generics can be nested so we start with the last one first as they might contain other nested replacements

            foreach (var entry in replaceDict.OrderByDescending(r => r.Key))
            {
                strs = strs.Select(s => s.Replace(ReplaceKey(entry.Key), entry.Value));
            }

            return strs;
        }
    }

    private class ParenthesisMasker : MaskerBase
    {
        private static Regex regex = new Regex(@"\([^)]+\)");
        protected override Regex Regex => regex;

        protected override string ReplaceKey(int key) => $";${key};";
    }

}
