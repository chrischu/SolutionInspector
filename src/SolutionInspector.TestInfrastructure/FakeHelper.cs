using System;
using FakeItEasy;

namespace SolutionInspector.TestInfrastructure
{
  public static class FakeHelper
  {
    public static T CreateAndConfigure<T>(Action<T> configureAction)
    {
      var fake = A.Fake<T>();
      configureAction(fake);
      return fake;
    }
  }
}