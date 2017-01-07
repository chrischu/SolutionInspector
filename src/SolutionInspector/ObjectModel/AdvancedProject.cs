using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Build.Construction;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
  internal class AdvancedProject : IAdvancedProject
  {
    private readonly Project _project;

    public AdvancedProject (Project project, Microsoft.Build.Evaluation.Project msBuildProject, ProjectInSolution msBuildProjectInSolution)
    {
      MsBuildProjectInSolution = msBuildProjectInSolution;
      _project = project;
      MsBuildProject = msBuildProject;

      Properties = ProcessProperties(MsBuildProject.Xml.Properties);
    }

    public ProjectInSolution MsBuildProjectInSolution { get; }
    public Microsoft.Build.Evaluation.Project MsBuildProject { get; }

    public IReadOnlyDictionary<string, IProjectProperty> Properties { get; }

    public IReadOnlyDictionary<string, IEvaluatedProjectPropertyValue> EvaluateProperties (
      BuildConfiguration configuration,
      Dictionary<string, string> propertyValues = null)
    {
      propertyValues = propertyValues ?? new Dictionary<string, string>();
      propertyValues.Add("Configuration", configuration.ConfigurationName);
      propertyValues.Add("Platform", configuration.PlatformName);
      return EvaluateProperties(propertyValues);
    }

    public IReadOnlyDictionary<string, IEvaluatedProjectPropertyValue> EvaluateProperties (Dictionary<string, string> propertyValues = null)
    {
      var result = new Dictionary<string, IEvaluatedProjectPropertyValue>();

      using (new MsBuildConditionContext(MsBuildProject, propertyValues))
      {
        foreach (var property in Properties.Values)
        {
          var projectPropertyElement = MsBuildProject.GetProperty(property.Name)?.Xml;
          if (projectPropertyElement != null)
            result.Add(
              property.Name,
              new EvaluatedProjectPropertyValue(projectPropertyElement.Value, new ProjectPropertyOccurrence(projectPropertyElement)));
        }
      }

      return result;
    }

    private IReadOnlyDictionary<string, IProjectProperty> ProcessProperties (IEnumerable<ProjectPropertyElement> propertyElements)
    {
      var result = new Dictionary<string, ProjectProperty>();

      foreach (var propertyElement in propertyElements)
      {
        ProjectProperty property;

        if (!result.TryGetValue(propertyElement.Name, out property))
          property = result[propertyElement.Name] = new ProjectProperty(propertyElement.Name, MsBuildProject.GetPropertyValue(propertyElement.Name));

        property.Add(new ProjectPropertyOccurrence(propertyElement));
      }

      return new ReadOnlyDictionary<string, IProjectProperty>(result.ToDictionary(x => x.Key, x => (IProjectProperty) x.Value));
    }

    private class MsBuildConditionContext : IDisposable
    {
      private readonly Microsoft.Build.Evaluation.Project _msBuildProject;
      private readonly List<string> _setPropertyNames = new List<string>();

      public MsBuildConditionContext (Microsoft.Build.Evaluation.Project msBuildProject, Dictionary<string, string> propertyValues)
      {
        _msBuildProject = msBuildProject;
        foreach (var propertyValue in propertyValues)
        {
          _setPropertyNames.Add(propertyValue.Key);
          _msBuildProject.SetGlobalProperty(propertyValue.Key, propertyValue.Value);
        }

        _msBuildProject.ReevaluateIfNecessary();
      }

      public void Dispose ()
      {
        foreach (var propertyName in _setPropertyNames)
          _msBuildProject.RemoveGlobalProperty(propertyName);

        _msBuildProject.ReevaluateIfNecessary();
      }
    }
  }
}