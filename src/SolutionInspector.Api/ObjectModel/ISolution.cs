using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Attributes;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a VisualStudio solution.
  /// </summary>
  [PublicApi]
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
}