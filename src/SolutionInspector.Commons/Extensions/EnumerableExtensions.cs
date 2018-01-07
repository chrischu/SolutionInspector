using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace SolutionInspector.Commons.Extensions
{
  /// <summary>
  ///   Extension methods for <see cref="IEnumerable{T}" />.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    ///   Checks if the given <paramref name="enumerable" /> contains more than one elements.
    /// </summary>
    public static bool ContainsMoreThanOne<T> (this IEnumerable<T> enumerable)
    {
      return enumerable.ContainsMoreThan(1);
    }

    /// <summary>
    ///   Checks if the given <paramref name="enumerable" /> contains more than <paramref name="count" /> elements.
    /// </summary>
    private static bool ContainsMoreThan<T> (this IEnumerable<T> enumerable, int count)
    {
      return enumerable.Skip(count).Any();
    }

    /// <summary>
    ///   Executes <paramref name="action" /> for every element in <paramref name="source" />.
    /// </summary>
    public static void ForEach<T> (this IEnumerable<T> source, Action<T> action)
    {
      foreach (var item in source)
        action(item);
    }

    /// <summary>
    ///   Joins all the string in the <paramref name="source" /> with the given <paramref name="separator" />.
    /// </summary>
    public static string Join (this IEnumerable<string> source, string separator)
    {
      return string.Join(separator, source);
    }

    /// <summary>
    ///   Stringifies the <paramref name="source" /> by converting every element to a string with <paramref name="converter" /> and then joining them with
    ///   a the <paramref name="separator" />.
    /// </summary>
    public static string ConvertAndJoin<TSource> (this IEnumerable<TSource> source, Func<TSource, object> converter = null, string separator = "")
    {
      converter = converter ?? (x => x);
      return source.Select(x => converter(x).ToString()).Join(separator);
    }

    private static readonly ThreadLocal<int> s_indentLevel = new ThreadLocal<int>();

    public static string FormatAsList<T> (this IEnumerable<T> source, [CanBeNull] string header, Func<T, string> formatElement)
    {
      return FormatAsListInternal(source, header, formatElement);
    }

    public static string FormatAsList (this IEnumerable<string> source, string header = null)
    {
      return FormatAsListInternal(source, header, x => x);
    }

    private static string FormatAsListInternal<T> (IEnumerable<T> source, [CanBeNull] string header, Func<T, string> formatElement)
    {
      try
      {
        s_indentLevel.Value++;

        var preamble = header == null
            ? ""
            : $"{header}:{Environment.NewLine}";

        return $"{preamble}{source.ConvertAndJoin(x => Indent(formatElement(x), s_indentLevel.Value), Environment.NewLine)}";
      }
      finally
      {
        s_indentLevel.Value--;
      }

      string Indent (string s, int level)
      {
        return new string(' ', level * 2) + "- " + s;
      }
    }
  }
}