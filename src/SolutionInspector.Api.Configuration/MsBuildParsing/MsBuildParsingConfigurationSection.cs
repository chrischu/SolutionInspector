using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.MsBuildParsing;

namespace SolutionInspector.Api.Configuration.MsBuildParsing
{
  /// <summary>
  ///   Configuration that controls how the MSBuild (e.g. csproj) files are parsed.
  /// </summary>
  [PublicAPI]
  public interface IMsBuildParsingConfiguration
  {
    /// <summary>
    ///   Returns <c>true</c> if the given <paramref name="projectItemType" /> is a valid project item type.
    /// </summary>
    bool IsValidProjectItemType (string projectItemType);
  }

  internal class MsBuildParsingConfigurationSection : ConfigurationElement, IMsBuildParsingConfiguration
  {
    private Lazy<HashSet<string>> _projectBuildActionsHashSet;

    internal const string ExampleConfiguration = @"<solutionInspector>
  <projectBuildActions>
    <projectBuildAction name=""None"" />
  </projectBuildActions>
</solutionInspector>";

    public MsBuildParsingConfigurationSection ()
    {
      _projectBuildActionsHashSet = new Lazy<HashSet<string>>(() => new HashSet<string>(ProjectBuildActions.Select(a => a.Name)));
    }

    [ConfigurationCollection (ElementName = "projectBuildAction", IsOptional = false)]
    public ConfigurationElementCollection<ProjectBuildActionConfigurationElement> ProjectBuildActions
        => GetConfigurationCollection<ProjectBuildActionConfigurationElement>();

    public bool IsValidProjectItemType (string projectItemType)
    {
      return _projectBuildActionsHashSet.Value.Contains(projectItemType);
    }
  }
}