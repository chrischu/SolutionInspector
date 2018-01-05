using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Attributes;

namespace SolutionInspector.Api.Configuration
{
  /// <inheritdoc />
  [ConfigurationConverter(typeof(NameFilterConverter))]
  public sealed class NameFilter : INameFilter
  {
    private static readonly Regex s_removeDuplicateAsterisksRegex = new Regex(@"\*+", RegexOptions.Compiled);

    private readonly Regex[] _excludeFilters;
    private readonly Regex[] _includeFilters;

    /// <inheritdoc />
    public NameFilter(IEnumerable<string> includes, IEnumerable<string> excludes = null)
    {
      excludes = excludes ?? new string[0];

      Includes = includes as IReadOnlyCollection<string> ?? includes.ToList();
      Excludes = excludes as IReadOnlyCollection<string> ?? excludes.ToList();

      var originalIncludes = Includes;
      Includes = Includes.Except(Excludes).ToList();
      Excludes = Excludes.Except(originalIncludes).ToList();

      Includes = SimplifyFilters(Includes).ToList();
      if (Includes.Count == 0)
        Includes = new[] { "*" };

      Includes = Includes;
      Excludes = SimplifyFilters(Excludes).ToList();

      _includeFilters = Includes.Select(FilterToRegex).ToArray();
      _excludeFilters = Excludes.Select(FilterToRegex).ToArray();
    }

    /// <summary>
    ///   All the includes in the filter.
    /// </summary>
    public IReadOnlyCollection<string> Includes { get; }

    /// <summary>
    ///   All the excludes in the filter.
    /// </summary>
    public IReadOnlyCollection<string> Excludes { get; }

    /// <summary>
    /// <see langword="true" /> if the filter includes all possible files, <see langword="false" /> otherwise.
    /// </summary>
    public bool IncludesAll => Includes.Count == 1 && Includes.Single() == "*";

    /// <summary>
    /// Returns <see langword="true" /> if the given <paramref name="name"/> matches the filter, <see langword="false" /> otherwise.
    /// </summary>
    public bool IsMatch(string name)
    {
      return _includeFilters.Any(f => f.IsMatch(name)) && !_excludeFilters.Any(f => f.IsMatch(name));
    }

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public override string ToString()
    {
      return string.Join(";", Includes.Select(s => "+" + s).Concat(Excludes.Select(s => "-" + s)));
    }

    private Regex FilterToRegex(string filter)
    {
      return new Regex($"^{filter.Replace(".", "\\.").Replace("*", ".*")}$", RegexOptions.None);
    }

    /// <summary>
    ///   Creates a <see cref="NameFilter" /> from the given <paramref name="filter" /> containing a <see cref="string" /> representation of a
    ///   <see cref="NameFilter" />.
    /// </summary>
    public static NameFilter Parse([NotNull] string filter)
    {
      var partRegex = @"((\+?|-)[\w\-.*]+)";
      var regex = new Regex($"^{partRegex}(;{partRegex})*$");

      if (!regex.IsMatch(filter))
        throw new FormatException($"The filter string '{filter}' is not in the correct format.");

      var parts = filter.Split(';');

      var includes = new List<string>();
      var excludes = new List<string>();

      foreach (var part in parts)
        if (part[0] == '-')
          excludes.Add(part.TrimStart('-'));
        else
          includes.Add(part.TrimStart('+'));

      return new NameFilter(includes, excludes);
    }

    private IEnumerable<string> SimplifyFilters(IReadOnlyCollection<string> filters)
    {
      filters = filters.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s_removeDuplicateAsterisksRegex.Replace(s, "*")).Distinct().ToList();

      var wildCardFilters = filters.Where(f => f.Contains("*")).Select(f => new { Regex = FilterToRegex(f), Filter = f });

      return wildCardFilters
          .Aggregate(
            filters,
            (current, wildCardFilter) => current.Where(f => f == wildCardFilter.Filter || !wildCardFilter.Regex.IsMatch(f)).ToList());
    }

    /// <inheritdoc />
    public override bool Equals([CanBeNull] object obj)
    {
      return obj is NameFilter other && ToString().Equals(other.ToString());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return ToString().GetHashCode();
    }
  }
}