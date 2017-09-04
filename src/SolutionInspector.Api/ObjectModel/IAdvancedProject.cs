using System;
using System.Collections.Generic;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Provides access to more advanced/raw properties of the underlying MSBuild project file.
  /// </summary>
  [PublicApi]
  public interface IAdvancedProject
  {
    /// <summary>
    ///   The raw <see cref="ProjectInSolution" />.
    /// </summary>
    ProjectInSolution MsBuildProjectInSolution { get; }

    /// <summary>
    ///   The raw <see cref="Microsoft.Build.Evaluation.Project" />.
    /// </summary>
    Project MsBuildProject { get; }

    /// <summary>
    ///   A collection of all project properties as they appear in the project file.
    /// </summary>
    IReadOnlyDictionary<string, IProjectProperty> Properties { get; }

    /// <summary>
    ///   Evaluates all the properties with the given <paramref name="configuration" /> and <paramref name="propertyValues" />.
    ///   Only properties with a true <see cref="IProjectPropertyCondition" /> are included.
    /// </summary>
    IReadOnlyDictionary<string, IEvaluatedProjectPropertyValue> EvaluateProperties (
      BuildConfiguration configuration,
      Dictionary<string, string> propertyValues = null);

    /// <summary>
    ///   Evaluates all the properties with the given <paramref name="propertyValues" />.
    ///   Only properties with a true <see cref="IProjectPropertyCondition" /> are included.
    /// </summary>
    IReadOnlyDictionary<string, IEvaluatedProjectPropertyValue> EvaluateProperties (Dictionary<string, string> propertyValues);
  }
}