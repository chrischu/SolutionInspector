using System;
using System.Collections.Generic;
using FluentAssertions;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  public static class DictionaryAssertionExtensions
  {
    public static void ShouldNotContainKey<TKey, TValue> (this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
      TValue value;
      dictionary.TryGetValue(key, out value).Should().BeFalse();
    }
  }
}