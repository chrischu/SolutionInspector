using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SolutionInspector.Api;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.Internals
{
  /// <summary>
  ///   Utility class to generate a <see cref="INameFilter" /> from a set of available names and a set of selected names.
  /// </summary>
  public static class NameFilterGenerator
  {
    /// <summary>
    ///   Generates a <see cref="INameFilter" /> from the given set of <paramref name="names" />.
    /// </summary>
    public static INameFilter Generate (IReadOnlyCollection<Tuple<string, bool>> names)
    {
      Trace.Assert(names.Select(x => x.Item1).Distinct().Count() == names.Count, "Names can only be passed once.");

      var filtered = names.Where(n => !string.IsNullOrWhiteSpace(n.Item1)).ToList();
      var availableNames = filtered.Select(n => n.Item1).ToList();
      var selectedNames = filtered.Where(n => n.Item2).Select(n => n.Item1).ToList();
      var unselectedNames = filtered.Where(n => !n.Item2).Select(n => n.Item1).ToList();

      if (availableNames.Count == selectedNames.Count)
        return new NameFilter(new[] { "*" });

      if (selectedNames.Count == 0)
        return new NameFilter(new string[0], new[] { "*" });

      var possibleNameFilters = GetPossibleNameFilters(selectedNames).ToArray();
      var nameFilters = possibleNameFilters.SelectMany(
        nameFilter =>
        {
          var excludes = unselectedNames.Where(nameFilter.IsMatch).ToList();

          if (excludes.Count == 0)
            return new[] { nameFilter };

          var possibleExcludeFilters = GetPossibleNameFilters(excludes);
          var filteredPossibleExcludeFilters = possibleExcludeFilters.Where(f => selectedNames.All(x => !f.IsMatch(x)));

          return filteredPossibleExcludeFilters.Select(f => new NameFilter(nameFilter.Includes, f.Includes));
        }).ToList();
      return nameFilters
          .OrderBy(f => f.Includes.Count + f.Excludes.Count)
          .ThenBy(f => f.Includes.Count(s => s.Contains("*")) + f.Excludes.Count(s => s.Contains("*")))
          .First();
    }

    private static IEnumerable<NameFilter> GetPossibleNameFilters(IReadOnlyCollection<string> elements)
    {
      yield return new NameFilter(elements);
      yield return new NameFilter(new[] { "*" });

      var possibleGroupings = GetPossibleGroupings(elements);

      foreach (var possibleGrouping in possibleGroupings)
        foreach (var nameFilter in GetOptimalNameFiltersForGrouping(possibleGrouping))
          yield return nameFilter;
    }

    private static IEnumerable<IEnumerable<IEnumerable<string>>> GetPossibleGroupings(IReadOnlyCollection<string> elements)
    {
      var maxLength = elements.Max(s => s.Length);

      var possibleGroupings =
          new HashSet<IEnumerable<IEnumerable<string>>>(new CollectionEqualityComparer<IEnumerable<string>>(new CollectionEqualityComparer<string>()));

      for (var prefixLength = 0; prefixLength <= maxLength; prefixLength++)
        for (var suffixLength = 0; suffixLength + prefixLength <= maxLength; suffixLength++)
        {
          var group = elements.GroupBy(e => e.Prefix(prefixLength) + e.Suffix(suffixLength)).Select(x => x.ToList()).ToList();
          if (group.Count != elements.Count)
            possibleGroupings.Add(group);
        }

      return possibleGroupings;
    }

    private static IEnumerable<NameFilter> GetOptimalNameFiltersForGrouping(IEnumerable<IEnumerable<string>> grouping)
    {
      grouping = grouping.Select(e => OptimizeGroup(e.ToList()).ToList()).ToList();

      var list = new HashSet<NameFilter>();
      BuildPossibleFilters(new string[0], grouping.ToList(), list);

      return list;
    }

    private static void BuildPossibleFilters(
      IReadOnlyCollection<string> includes,
      IReadOnlyCollection<IEnumerable<string>> possibleIncludes,
      ISet<NameFilter> result)
    {
      if (possibleIncludes.Any())
      {
        foreach (var x in possibleIncludes.First())
          BuildPossibleFilters(includes.Concat(new[] { x }).ToList(), possibleIncludes.Skip(1).ToList(), result);
      }
      else
        result.Add(new NameFilter(includes.ToList()));
    }

    private static IEnumerable<string> OptimizeGroup(IReadOnlyCollection<string> elements)
    {
      if (elements.Count == 1)
      {
        yield return elements.Single();
        yield break;
      }

      var longestCommonPrefix = GetLongestCommonAffix(elements, (s, i) => s.Prefix(i));
      var longestCommonSuffix = GetLongestCommonAffix(elements, (s, i) => s.Suffix(i));

      var foundCommonPrefix = !string.IsNullOrEmpty(longestCommonPrefix);
      var foundCommonSuffix = !string.IsNullOrEmpty(longestCommonSuffix);

      if (foundCommonPrefix)
        yield return longestCommonPrefix + "*";

      if (foundCommonSuffix)
        yield return "*" + longestCommonSuffix;

      if (foundCommonPrefix && foundCommonSuffix)
        yield return longestCommonPrefix + "*" + longestCommonSuffix;
    }

    [ExcludeFromCodeCoverage]
    private static string GetLongestCommonAffix(
      IReadOnlyCollection<string> elements,
      Func<string, int, string> affixFunc)
    {
      var maxLength = elements.Max(s => s.Length);

      for (var affixLength = 1; affixLength <= maxLength; affixLength++)
      {
        var grouped = elements.GroupBy(e => affixFunc(e, affixLength)).ToList();
        if (grouped.Count > 1)
          return affixFunc(elements.First(), affixLength - 1);
      }

      throw new InvalidOperationException("Unreachable code");
    }
  }
}