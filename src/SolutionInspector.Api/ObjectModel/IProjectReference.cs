using System;
using JetBrains.Annotations;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a reference to another <see cref="IProject"/> in the same solution.
  /// </summary>
  [PublicAPI]
  public interface IProjectReference
  {
    /// <summary>
    ///   The referenced project.
    /// </summary>
    IProject Project { get; }

    /// <summary>
    /// The original MSBuild project item representing the reference.
    /// </summary>
    Microsoft.Build.Evaluation.ProjectItem OriginalProjectItem { get; }

    /// <summary>
    /// The include pointing to the referenced .csproj file.
    /// </summary>
    string Include { get; }

    /// <summary>
    ///   A <see cref="IFileInfo" /> that represents the referenced project file.
    /// </summary>
    IFileInfo File { get; }

    /// <summary>
    /// The referenced project's GUID.
    /// </summary>
    Guid? ReferencedProjectGuid { get; }

    /// <summary>
    /// The referenced project's name.
    /// </summary>
    string ReferencedProjectName { get; }
  }
}