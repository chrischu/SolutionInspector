using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers;

namespace SolutionInspector.Internals.ObjectModel
{
  internal sealed class Solution : ISolution
  {
    private readonly string _solutionPath;

    private Solution (string solutionPath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      _solutionPath = solutionPath;
      Name = Path.GetFileNameWithoutExtension(solutionPath);
      SolutionDirectory = Wrapper.Wrap(new DirectoryInfo(Path.GetDirectoryName(solutionPath).AssertNotNull()));
      var solutionFile = SolutionFile.Parse(solutionPath);

      Projects =
          solutionFile.ProjectsInOrder.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
              .Select(p => Project.FromSolution(this, p, msBuildParsingConfiguration))
              .ToArray();
      BuildConfigurations = solutionFile.SolutionConfigurations.Select(c => new BuildConfiguration(c.ConfigurationName, c.PlatformName)).ToArray();
    }

    public string Name { get; }
    public IDirectoryInfo SolutionDirectory { get; }
    public IReadOnlyCollection<IProject> Projects { get; }
    public IReadOnlyCollection<BuildConfiguration> BuildConfigurations { get; }

    [CanBeNull]
    public IProject GetProjectByProjectGuid (Guid projectGuid)
    {
      return Projects.SingleOrDefault(p => p.Guid == projectGuid);
    }

    [CanBeNull]
    public IProject GetProjectByAbsoluteProjectFilePath (string absoluteProjectPath)
    {
      return Projects.SingleOrDefault(p => p.ProjectFile.FullName == absoluteProjectPath);
    }

    string IRuleTarget.Identifier => Path.GetFileName(_solutionPath);
    string IRuleTarget.FullPath => _solutionPath;

    public void Dispose ()
    {
      foreach (var project in Projects)
        project.Dispose();
    }

    public static Solution Load (string solutionFilePath, IMsBuildParsingConfiguration msBuildParsingConfiguration)
    {
      var fullPath = Path.GetFullPath(solutionFilePath);
      return new Solution(fullPath, msBuildParsingConfiguration);
    }
  }
}