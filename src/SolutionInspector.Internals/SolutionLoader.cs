using JetBrains.Annotations;
using Microsoft.Build.Exceptions;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Exceptions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Internals.ObjectModel;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Internals
{
  /// <summary>
  ///   Utility to load solutions.
  /// </summary>
  public interface ISolutionLoader
  {
    /// <summary>
    ///   Loads and parses a solution to get a <see cref="ISolution" /> from the given <paramref name="solutionPath" />.
    /// </summary>
    /// <exception cref="SolutionNotFoundException" />
    /// <exception cref="InvalidProjectFileException" />
    ISolution Load (string solutionPath, IMsBuildParsingConfiguration msBuildParsingConfiguration);
  }

  /// <inheritdoc />
  [UsedImplicitly /* by container */]
  public class SolutionLoader : ISolutionLoader
  {
    private readonly IFileStatic _file;

    /// <summary>
    /// Creates a <see cref="SolutionLoader"/>.
    /// </summary>
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