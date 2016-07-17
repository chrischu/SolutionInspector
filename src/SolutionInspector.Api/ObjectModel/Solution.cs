using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers.IO;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a VisualStudio solution.
  /// </summary>
  [PublicAPI]
  public interface ISolution : IRuleTarget, IDisposable
  {
    /// <summary>
    ///   The solution's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The directory where the solution lies.
    /// </summary>
    IDirectoryInfo SolutionDirectory { get; }

    /// <summary>
    ///   All <see cref="IProject" /> contained in the solution.
    /// </summary>
    IReadOnlyCollection<IProject> Projects { get; }

    /// <summary>
    ///   All <see cref="BuildConfiguration" />s in the solution.
    /// </summary>
    IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    /// <summary>
    ///   Returns the project with a matching <paramref name="projectGuid" /> or <see langword="null" /> if no such project can be found.
    /// </summary>
    [CanBeNull]
    IProject GetProjectByProjectGuid (Guid projectGuid);

    /// <summary>
    ///   Returns the project point to by the given <paramref name="absoluteProjectPath" /> or <see langword="null" /> if no such project can be found.
    /// </summary>
    [CanBeNull]
    IProject GetProjectByAbsoluteProjectFilePath (string absoluteProjectPath);
  }

  internal sealed class Solution : ISolution
  {
    private readonly string _solutionPath;
    private readonly SolutionFile _solutionFile;

    private Solution (string solutionPath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      _solutionPath = solutionPath;
      Name = Path.GetFileNameWithoutExtension(solutionPath);
      SolutionDirectory = new DirectoryInfoWrapper(new DirectoryInfo(Path.GetDirectoryName(solutionPath).AssertNotNull()));
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

    public IProject GetProjectByProjectGuid (Guid projectGuid)
    {
      return Projects.SingleOrDefault(p => p.Guid == projectGuid);
    }

    public IProject GetProjectByAbsoluteProjectFilePath (string absoluteProjectPath)
    {
      return Projects.SingleOrDefault(p => p.ProjectFile.FullName == absoluteProjectPath);
    }

    string IRuleTarget.Identifier => Path.GetFileName(_solutionPath);
    string IRuleTarget.FullPath => _solutionPath;

    public static Solution Load (string solutionFilePath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      var fullPath = Path.GetFullPath(solutionFilePath);
      return new Solution(fullPath, msBuildParsingConfiguration);
    }

    public void Dispose ()
    {
      foreach (var project in Projects)
        project.Dispose();
    }
  }
}