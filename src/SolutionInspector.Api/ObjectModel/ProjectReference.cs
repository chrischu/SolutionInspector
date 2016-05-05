using System;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// A reference to another project in the same solution.
  /// </summary>
  [PublicAPI]
  public class ProjectReference
  {
    /// <summary>
    /// The referenced project.
    /// </summary>
    public IProject Project { get; }

    /// <summary>
    /// Creates a new <see cref="ProjectReference"/>.
    /// </summary>
    public ProjectReference(IProject project)
    {
      Project = project;
    }
  }
}