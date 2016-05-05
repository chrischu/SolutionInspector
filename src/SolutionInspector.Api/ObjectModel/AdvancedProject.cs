using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Build.Construction;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Provides access to more advanced/raw properties of the underlying MSBuild project file.
  /// </summary>
  [PublicAPI]
  public interface IAdvancedProject
  {
    /// <summary>
    ///   The raw <see cref="ProjectInSolution" />.
    /// </summary>
    ProjectInSolution MsBuildProjectInSolution { get; }

    /// <summary>
    ///   The raw <see cref="Microsoft.Build.Evaluation.Project" />.
    /// </summary>
    Microsoft.Build.Evaluation.Project MsBuildProject { get; }

    /// <summary>
    ///   A raw collection of all project properties.
    /// </summary>
    IReadOnlyDictionary<string, string> Properties { get; }

    /// <summary>
    ///   A raw collection of all configuration-dependent (i.e. properties that depend on the configuration/platform of the build) project properties.
    /// </summary>
    IReadOnlyDictionary<BuildConfiguration, IReadOnlyDictionary<string, string>> ConfigurationDependentProperties { get; }

    /// <summary>
    ///   A raw collection of all conditional project properties.
    /// </summary>
    IReadOnlyDictionary<string, ConditionalProperty> ConditionalProperties { get; }
  }

  [PublicAPI]
  internal class AdvancedProject : IAdvancedProject
  {
    private const string c_configurationRegex = @"^\s*'\$\(Configuration\)\|\$\(Platform\)'\s*==\s*'(?<Configuration>\w+)\|(?<Platform>\w+)'\s*$";
    private readonly Project _project;

    public AdvancedProject (Project project, Microsoft.Build.Evaluation.Project msBuildProject, ProjectInSolution msBuildProjectInSolution)
    {
      MsBuildProjectInSolution = msBuildProjectInSolution;
      _project = project;
      MsBuildProject = msBuildProject;

      var projectProperties =
          MsBuildProject.Properties.Where(p => !p.IsReservedProperty && !p.IsEnvironmentProperty && !p.IsGlobalProperty && !p.IsImported).ToArray();

      var unconditionalProperties = projectProperties.Where(p => p.Xml.Parent.Condition == "" && p.Xml.Condition == "");
      Properties = unconditionalProperties.ToDictionary(p => p.Name, p => p.EvaluatedValue);

      var conditionalProperties = projectProperties.Where(p => p.Xml.Parent.Condition != "" && p.Xml.Condition != "")
          .Select(p => new { Property = p, Condition = p.Xml.Condition != "" ? p.Xml.Condition : p.Xml.Parent.Condition }).ToArray();

      ConfigurationDependentProperties = CreateConfigurationDependentProperties();

      ConditionalProperties = conditionalProperties
          .Where(p => !Regex.IsMatch(p.Condition, c_configurationRegex))
          .ToDictionary(p => p.Property.Name, p => new ConditionalProperty(p.Property.EvaluatedValue, p.Condition));
    }

    public ProjectInSolution MsBuildProjectInSolution { get; }
    public Microsoft.Build.Evaluation.Project MsBuildProject { get; }

    public IReadOnlyDictionary<string, string> Properties { get; }

    public IReadOnlyDictionary<BuildConfiguration, IReadOnlyDictionary<string, string>> ConfigurationDependentProperties { get; }

    public IReadOnlyDictionary<string, ConditionalProperty> ConditionalProperties { get; }

    private IReadOnlyDictionary<BuildConfiguration, IReadOnlyDictionary<string, string>> CreateConfigurationDependentProperties ()
    {
      var dict = new Dictionary<BuildConfiguration, IReadOnlyDictionary<string, string>>();

      var configurationDependentProperties = MsBuildProject.Xml.PropertyGroups
          .Select(g => new { PropertyGroup = g, Match = Regex.Match(g.Condition, c_configurationRegex) })
          .Where(x => x.Match.Success)
          .Select(
              x =>
                  new
                  {
                      Configuration = new BuildConfiguration(x.Match.Groups["Configuration"].Value, x.Match.Groups["Platform"].Value),
                      Properties = x.PropertyGroup.Children.Cast<ProjectPropertyElement>().Select(p => p.Name)
                  })
          .GroupBy(x => x.Configuration, x => x.Properties)
          // ReSharper disable once PossibleMultipleEnumeration
          .ToDictionary(x => x.Key, x => x.SelectMany(y => y));

      var previousConfiguration = MsBuildProject.GetPropertyValue("Configuration");
      var previousPlatform = MsBuildProject.GetPropertyValue("Platform");

      foreach (var configuration in _project.BuildConfigurations)
      {
        MsBuildProject.SetProperty("Configuration", configuration.ConfigurationName);
        MsBuildProject.SetProperty("Platform", configuration.PlatformName);
        MsBuildProject.ReevaluateIfNecessary();

        try
        {
          dict.Add(configuration, configurationDependentProperties[configuration].ToDictionary(s => s, s => MsBuildProject.GetPropertyValue(s)));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }

      MsBuildProject.SetProperty("Configuration", previousConfiguration);
      MsBuildProject.SetProperty("Platform", previousPlatform);
      MsBuildProject.ReevaluateIfNecessary();

      return dict;
    }
  }
}