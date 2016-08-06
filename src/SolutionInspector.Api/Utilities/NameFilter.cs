using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionInspector.Api.Utilities
{
  /// <summary>
  ///   Filter that checks a string against a set of include/exclude filters.
  /// </summary>
  public interface INameFilter
  {
    /// <summary>
    ///   Returns <c>true</c> if the given <paramref name="name" /> matches the filter, <c>false</c> otherwise.
    /// </summary>
    bool IsMatch(string name);
  }

  [TypeConverter (typeof(NameFilterConverter))]
  internal class NameFilter : INameFilter
  {
    private readonly Regex[] _includeFilters;
    private readonly Regex[] _excludeFilters;

    public NameFilter (IEnumerable<string> includes, IEnumerable<string> excludes = null)
    {
      _includeFilters = includes.Select(s => new Regex($"^{s.Replace("*", ".*")}$", RegexOptions.Compiled)).ToArray();
      _excludeFilters = (excludes ?? Enumerable.Empty<string>()).Select(s => new Regex($"^{s.Replace("*", ".*")}$", RegexOptions.Compiled)).ToArray();
    }

    public bool IsMatch (string name)
    {
      return _includeFilters.Any(f => f.IsMatch(name)) && !_excludeFilters.Any(f => f.IsMatch(name));
    }

    public override string ToString ()
    {
      var filters =
          _includeFilters.Select(r => "+" + r.ToString().TrimStart('^').TrimEnd('$').Replace(".*", "*"))
              .Concat(_excludeFilters.Select(r => "-" + r.ToString().TrimStart('^').TrimEnd('$').Replace(".*", "*")));

      return string.Join(";", filters);
    }
  }
}