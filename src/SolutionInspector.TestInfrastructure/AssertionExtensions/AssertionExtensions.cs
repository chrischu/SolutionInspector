using System;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  /// <summary>
  ///   Extensions to make assertions easier.
  /// </summary>
  public static class AssertionExtensions
  {
    /// <summary>
    ///   Allows grouping and naming multiple assertions for better test readability.
    /// </summary>
    // ReSharper disable once UnusedParameter.Global
    public static void Should<T> (this T obj, string description, Action<T> assertions)
    {
      assertions(obj);
    }
  }
}