using System;
using System.IO;
using System.Reflection;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.Internals.Tests.ObjectModel
{
  public static class TestDataHelper
  {
    public static string GetTestDataPath(string relativePath)
    {
      var binaryDirectory = Path.GetDirectoryName(new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath).AssertNotNull();
      return Path.Combine(binaryDirectory, "_TestData", relativePath);
    }
  }
}