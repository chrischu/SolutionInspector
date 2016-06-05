using System;
using System.Diagnostics.CodeAnalysis;

namespace SolutionInspector
{
  [ExcludeFromCodeCoverage]
  internal static class SolutionInspectorProgram
  {
    private static int Main (string[] args)
    {
      return Api.SolutionInspector.Run(args);
    }
  }
}