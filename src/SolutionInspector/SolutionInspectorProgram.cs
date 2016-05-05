using System;

namespace SolutionInspector
{
  internal static class SolutionInspectorProgram
  {
    private static int Main (string[] args)
    {
      return Api.SolutionInspector.Run(args);
    }
  }
}