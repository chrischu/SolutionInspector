using System;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   A reference to another project in the same solution.
  /// </summary>
  [PublicAPI]
  public class ProjectReference
  {
    /// <summary>
    ///   The referenced project.
    /// </summary>
    public IProject Project { get; }

    /// <summary>
    /// The original MSBuild project item representing the reference.
    /// </summary>
    public Microsoft.Build.Evaluation.ProjectItem OriginalProjectItem { get; }

    /// <summary>
    /// The include pointing to the referenced .csproj file.
    /// </summary>
    public string Include { get; }

    /// <summary>
    /// The referenced project's GUID.
    /// </summary>
    public Guid? ReferencedProjectGuid { get; }

    /// <summary>
    /// The referenced project's name.
    /// </summary>
    public string ReferencedProjectName { get; }

    /// <summary>
    ///   Creates a new <see cref="ProjectReference" />.
    /// </summary>
    public ProjectReference (Microsoft.Build.Evaluation.ProjectItem projectItem, [CanBeNull] IProject project)
    {
      Project = project;
      OriginalProjectItem = projectItem;
      Include = projectItem.EvaluatedInclude;

      Guid referencedProjectGuid;
      if (Guid.TryParse(projectItem.GetMetadataValue("Project"), out referencedProjectGuid))
        ReferencedProjectGuid = referencedProjectGuid;

      ReferencedProjectName = projectItem.GetMetadataValue("Name");
    }
  }
}