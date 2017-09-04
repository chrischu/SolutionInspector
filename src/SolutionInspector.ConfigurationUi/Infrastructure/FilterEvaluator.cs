using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace SolutionInspector.ConfigurationUi.Infrastructure
{
  internal interface IFilterEvaluator
  {
    bool MatchesFilter (string filter, string value);
  }

  internal class FilterEvaluator : IFilterEvaluator
  {
    private Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

    public bool MatchesFilter (string filter, string value)
    {
      var regex = GetFilterRegex(filter);

      return regex.IsMatch(value);
    }

    private Regex GetFilterRegex (string filter)
    {
      Regex regex;
      if (!_regexCache.TryGetValue(filter, out regex))
      {
        var filterRegexString = new string(GetFilterRegexString(filter).ToArray());
        regex = _regexCache[filter] = new Regex(filterRegexString.Replace("*", ".*"), RegexOptions.IgnoreCase);
      }

      return regex;
    }

    [SuppressMessage ("ReSharper", "PossibleMultipleEnumeration")]
    private IEnumerable<char> GetFilterRegexString (IEnumerable<char> filter)
    {
      yield return filter.First();

      foreach (var ch in filter.Skip(1))
      {
        if (char.IsUpper(ch))
          yield return '*';

        yield return ch;
      }
    }
  }
}