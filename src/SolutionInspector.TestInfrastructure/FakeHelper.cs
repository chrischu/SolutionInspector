using System;
using FakeItEasy;

namespace SolutionInspector.TestInfrastructure
{
  /// <summary>
  ///   Helper to create and immediately configure a fake object.
  /// </summary>
  public static class FakeHelper
  {
    public static T CreateAndConfigure<T> (Action<T> configureAction)
    {
      return A.Fake<T>(o => o.ConfigureFake(configureAction));
    }
  }
}