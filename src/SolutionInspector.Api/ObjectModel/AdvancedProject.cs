using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    ///   A collection of all (unconditional) project properties.
    /// </summary>
    IReadOnlyDictionary<string, IProjectProperty> Properties { get; }

    /// <summary>
    ///   A collection of all conditional project properties.
    /// </summary>
    IReadOnlyCollection<IConditionalProjectProperty> ConditionalProperties { get; }

    /// <summary>
    ///   Gets all properties active under the given <paramref name="configuration" /> and when the <paramref name="properties" /> are set to the
    ///   specified values.
    /// </summary>
    IReadOnlyDictionary<string, IProjectProperty> GetPropertiesBasedOnCondition (
        BuildConfiguration configuration,
        Dictionary<string, string> properties = null);

    /// <summary>
    ///   Gets all properties active when the <paramref name="properties" /> are set to the specified values.
    /// </summary>
    IReadOnlyDictionary<string, IProjectProperty> GetPropertiesBasedOnCondition (Dictionary<string, string> properties);
  }

  [PublicAPI]
  internal class AdvancedProject : IAdvancedProject
  {
    private readonly Project _project;

    public AdvancedProject (Project project, Microsoft.Build.Evaluation.Project msBuildProject, ProjectInSolution msBuildProjectInSolution)
    {
      MsBuildProjectInSolution = msBuildProjectInSolution;
      _project = project;
      MsBuildProject = msBuildProject;

      var projectProperties = MsBuildProject.Xml.Properties.ToArray();
      var classifiedProperties = ClassifyProperties(projectProperties);

      Properties = new ReadOnlyDictionary<string, IProjectProperty>(classifiedProperties.UnconditionalProperties.ToDictionary(p => p.Name));
      ConditionalProperties = classifiedProperties.ConditionalProperties;
    }

    private ClassifiedProperties ClassifyProperties (IReadOnlyCollection<ProjectPropertyElement> properties)
    {
      var unconditionalProperties = new List<ProjectProperty>();
      var conditionalProperties = new List<ConditionalProjectProperty>();

      foreach (var property in properties)
      {
        if (string.IsNullOrWhiteSpace(property.Condition) && string.IsNullOrWhiteSpace(property.Parent?.Condition))
          unconditionalProperties.Add(new ProjectProperty(property));
        else
          conditionalProperties.Add(new ConditionalProjectProperty(property));
      }

      return new ClassifiedProperties(unconditionalProperties, conditionalProperties);
    }

    public ProjectInSolution MsBuildProjectInSolution { get; }
    public Microsoft.Build.Evaluation.Project MsBuildProject { get; }

    public IReadOnlyDictionary<string, IProjectProperty> Properties { get; }
    public IReadOnlyCollection<IConditionalProjectProperty> ConditionalProperties { get; }

    public IReadOnlyDictionary<string, IProjectProperty> GetPropertiesBasedOnCondition (
        BuildConfiguration configuration,
        Dictionary<string, string> properties = null)
    {
      properties = properties ?? new Dictionary<string, string>();
      properties.Add("Configuration", configuration.ConfigurationName);
      properties.Add("Platform", configuration.PlatformName);
      return GetPropertiesBasedOnCondition(properties);
    }

    public IReadOnlyDictionary<string, IProjectProperty> GetPropertiesBasedOnCondition (Dictionary<string, string> properties)
    {
      var result = new Dictionary<string, IProjectProperty>();

      using (new MsBuildConditionContext(MsBuildProject, properties))
      {
        foreach (var property in ConditionalProperties)
        {
          var projectPropertyElement = MsBuildProject.GetProperty(property.Name)?.Xml;
          // TODO: Remove the ContainsKey workaround after figuring out a good way to handle conditional properties.
          if (projectPropertyElement != null && !result.ContainsKey(property.Name))
            result.Add(property.Name, new ProjectProperty(projectPropertyElement));
        }
      }

      return result;
    }

    private class MsBuildConditionContext : IDisposable
    {
      private readonly Microsoft.Build.Evaluation.Project _msBuildProject;
      private readonly Dictionary<string, string> _previousPropertyValues = new Dictionary<string, string>();

      public MsBuildConditionContext (Microsoft.Build.Evaluation.Project msBuildProject, Dictionary<string, string> propertyValues)
      {
        _msBuildProject = msBuildProject;
        foreach (var propertyValue in propertyValues)
        {
          _previousPropertyValues.Add(propertyValue.Key, _msBuildProject.GetPropertyValue(propertyValue.Key));
          _msBuildProject.SetProperty(propertyValue.Key, propertyValue.Value);
        }

        _msBuildProject.ReevaluateIfNecessary();
      }

      public void Dispose ()
      {
        foreach (var propertyValue in _previousPropertyValues)
          _msBuildProject.SetProperty(propertyValue.Key, propertyValue.Value);

        _msBuildProject.ReevaluateIfNecessary();
      }
    }

    private class ClassifiedProperties
    {
      public IReadOnlyCollection<IProjectProperty> UnconditionalProperties { get; }
      public IReadOnlyCollection<IConditionalProjectProperty> ConditionalProperties { get; }

      public ClassifiedProperties (
          IReadOnlyCollection<IProjectProperty> unconditionalProperties,
          IReadOnlyCollection<IConditionalProjectProperty> conditionalProperties)
      {
        UnconditionalProperties = unconditionalProperties;
        ConditionalProperties = conditionalProperties;
      }
    }
  }
}