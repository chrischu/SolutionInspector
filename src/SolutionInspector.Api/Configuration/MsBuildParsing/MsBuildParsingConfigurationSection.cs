using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  /// <summary>
  ///   Configuration that controls how the MSBuild (e.g. csproj) files are parsed.
  /// </summary>
  [PublicApi]
  public interface IMsBuildParsingConfiguration
  {
    /// <summary>
    ///   Returns <c>true</c> if the given <paramref name="projectItemType" /> is a valid project item type.
    /// </summary>
    bool IsValidProjectItemType (string projectItemType);
  }

  internal class MsBuildParsingConfigurationSection : ConfigurationSection, IMsBuildParsingConfiguration
  {
    internal const string ExampleConfiguration = @"<solutionInspector>
  <projectBuildActions>
    <projectBuildAction name=""None"" />
  </projectBuildActions>
</solutionInspector>";

    private Lazy<HashSet<string>> _projectBuildActionsHashSet;

    public MsBuildParsingConfigurationSection ()
    {
      _projectBuildActionsHashSet = new Lazy<HashSet<string>>(() => new HashSet<string>(ProjectBuildActions.Select(a => a.Name)));
    }

    [ConfigurationProperty("projectBuildActions")]
    public ProjectBuildActionsConfigurationElement ProjectBuildActions => (ProjectBuildActionsConfigurationElement) this["projectBuildActions"];

    public bool IsValidProjectItemType (string projectItemType)
    {
      return _projectBuildActionsHashSet.Value.Contains(projectItemType);
    }
  }
}