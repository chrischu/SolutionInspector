using System.Diagnostics.CodeAnalysis;

namespace SolutionInspector
{
  [ExcludeFromCodeCoverage]
  internal static class SolutionInspectorProgram
  {
    public static int Main (string[] args)
    {
      return SolutionInspector.Run(args);
    }
  }
}