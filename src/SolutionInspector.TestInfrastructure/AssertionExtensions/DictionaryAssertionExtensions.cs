using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Collections;
using JetBrains.Annotations;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  /// <summary>
  ///   Extensions for easier assertion of dictionaries.
  /// </summary>
  public static class DictionaryAssertionExtensions
  {
    /// <summary>
    ///   Returns an <see cref="GenericDictionaryAssertions{TKey,TValue}" /> object that can be used to assert the
    ///   current <see cref="IReadOnlyDictionary{TKey,TValue}" />.
    /// </summary>
    [Pure]
    public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue> (this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
      return (dictionary as IDictionary<TKey, TValue>).Should();
    }
  }
}