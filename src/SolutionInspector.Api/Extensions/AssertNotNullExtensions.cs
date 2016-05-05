using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Extensions
{
  /// <summary>
  ///   Extensions that make it more convenient to assert that a value is not <c>null</c>.
  /// </summary>
  public static class AssertNotNullExtensions
  {
    /// <summary>
    ///   Asserts that the <paramref name="obj" /> is not <c>null</c>.
    /// </summary>
    [AssertionMethod]
    [ContractAnnotation ("obj: null => halt")]
    [NotNull]
    public static T AssertNotNull<T> ([CanBeNull] this T obj, string message = null) where T : class
    {
      Trace.Assert(obj != null, message ?? "AssertNotNull failed");
      return obj;
    }
  }
}