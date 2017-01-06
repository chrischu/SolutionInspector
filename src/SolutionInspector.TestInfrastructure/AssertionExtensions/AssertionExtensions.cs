using System;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  public static class AssertionExtensions
  {
    // ReSharper disable once UnusedParameter.Global
    public static void Should<T>(this T obj, string description, Action<T> assertions)
    {
      assertions(obj);
    }
  }
}