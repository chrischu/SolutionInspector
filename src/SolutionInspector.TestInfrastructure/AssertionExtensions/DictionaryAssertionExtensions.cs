using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Collections;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  /// <summary>
  ///   Extensions for easier assertion of dictionaries.
  /// </summary>
  public static class DictionaryAssertionExtensions
  {
    public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue> (this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
      return (dictionary as IDictionary<TKey, TValue>).Should();
    }
  }
}