using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Configuration.MsBuildParsing;
using SolutionInspector.Rules;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a VisualStudio solution.
  /// </summary>
  [PublicAPI]
  public interface ISolution : IRuleTarget
  {
    /// <summary>
    /// The solution's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The directory where the solution lies.
    /// </summary>
    IDirectoryInfo SolutionDirectory { get; }

    /// <summary>
    /// All <see cref="IProject"/> contained in the solution.
    /// </summary>
    IReadOnlyCollection<IProject> Projects { get; }

    /// <summary>
    /// All <see cref="BuildConfiguration"/>s in the solution.
    /// </summary>
    IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }
  }

  internal sealed class Solution : ISolution
  {
    private readonly string _solutionPath;
    private readonly SolutionFile _solutionFile;

    private Solution(string solutionPath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      _solutionPath = solutionPath;
      Name = Path.GetFileNameWithoutExtension(solutionPath);
      SolutionDirectory = new DirectoryInfoWrap(Path.GetDirectoryName(solutionPath));
      _solutionFile = SolutionFile.Parse(solutionPath);

      Projects =
          _solutionFile.ProjectsInOrder.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
              .Select(p => Project.FromSolution(this, p, msBuildParsingConfiguration))
              .ToArray();
      BuildConfigurations = _solutionFile.SolutionConfigurations.Select(c => new BuildConfiguration(c.ConfigurationName, c.PlatformName)).ToArray();
    }

    public string Name { get; }
    public IDirectoryInfo SolutionDirectory { get; }
    public IReadOnlyCollection<IProject> Projects { get; }
    public IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    string IRuleTarget.Identifier => Path.GetFileName(_solutionPath);

    public static Solution Load(string solutionFilePath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      var fullPath = Path.GetFullPath(solutionFilePath);
      return new Solution(fullPath, msBuildParsingConfiguration);
    }
  }
}