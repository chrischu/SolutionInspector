using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Collections;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  public static class DictionaryAssertionExtensions
  {
    public static GenericDictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
      return (dictionary as IDictionary<TKey, TValue>).Should();
    }
  }
}