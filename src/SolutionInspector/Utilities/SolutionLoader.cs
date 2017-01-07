using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Utilities;
using SolutionInspector.ObjectModel;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Utilities
{
  internal interface ISolutionLoader
  {
    ISolution Load (string solutionPath, IMsBuildParsingConfiguration msBuildParsingConfiguration);
  }

  [UsedImplicitly /* by container */]
  internal class SolutionLoader : ISolutionLoader
  {
    private readonly IFileStatic _file;

    public SolutionLoader (IFileStatic file)
    {
      _file = file;
    }

    public ISolution Load (string solutionPath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      if (!_file.Exists(solutionPath))
        throw new SolutionNotFoundException(solutionPath);

      return Solution.Load(solutionPath, msBuildParsingConfiguration);
    }
  }
}