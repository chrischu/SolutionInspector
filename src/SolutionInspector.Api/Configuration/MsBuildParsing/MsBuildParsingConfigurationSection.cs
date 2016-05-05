using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Evaluation;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  /// <summary>
  /// Configuration that controls how the MSBuild (e.g. csproj) files are parsed.
  /// </summary>
  [PublicAPI]
  public interface IMsBuildParsingConfiguration
  {
    /// <summary>
    /// Collection of all the build actions applicable to a project item (e.g. None, Content, Compile).
    /// </summary>
    IReadOnlyCollection<string> ProjectBuildActions { get; }

    /// <summary>
    /// Returns <c>true</c> if the given <paramref name="projectItem"/> is a valid project item (i.e. its build action is contained in <see cref="ProjectBuildActions"/>.
    /// </summary>
    bool IsValidProjectItem(ProjectItem projectItem);
  }

  internal class MsBuildParsingConfigurationSection : ConfigurationSection, IMsBuildParsingConfiguration
  {
    private Lazy<HashSet<string>> _projectBuildActionsHashSet;

    internal const string ExampleConfiguration = @"<solutionInspector>
  <projectBuildActions>
    <projectBuildAction name=""None"" />
  </projectBuildActions>
</solutionInspector>";

    public MsBuildParsingConfigurationSection()
    {
      _projectBuildActionsHashSet = new Lazy<HashSet<string>>(() => new HashSet<string>(ProjectBuildActions.Select(a => a.Name)));
    }

    [ConfigurationProperty("projectBuildActions")]
    public ProjectBuildActionsConfigurationElement ProjectBuildActions => (ProjectBuildActionsConfigurationElement) this["projectBuildActions"];

    public bool IsValidProjectItem(ProjectItem projectItem)
    {
      return _projectBuildActionsHashSet.Value.Contains(projectItem.ItemType);
    }

    IReadOnlyCollection<string> IMsBuildParsingConfiguration.ProjectBuildActions => _projectBuildActionsHashSet.Value;

    public static MsBuildParsingConfigurationSection Load()
    {
      return (MsBuildParsingConfigurationSection) ConfigurationManager.GetSection("solutionInspector");
    }
  }
}