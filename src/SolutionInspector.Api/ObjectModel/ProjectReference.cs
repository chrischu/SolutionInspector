using System;
using System.IO;
using SystemInterface.IO;
using SystemWrapper.IO;
using JetBrains.Annotations;

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

  internal class ProjectReference : IProjectReference
  {
    public IProject Project { get; }
    public Microsoft.Build.Evaluation.ProjectItem OriginalProjectItem { get; }
    public string Include { get; }
    public IFileInfo File { get; }
    public Guid? ReferencedProjectGuid { get; }
    public string ReferencedProjectName { get; }

    public ProjectReference (ISolution solution, Microsoft.Build.Evaluation.ProjectItem projectItem)
    {
      OriginalProjectItem = projectItem;
      Include = projectItem.EvaluatedInclude;

      var fullPath = Path.GetFullPath(Path.Combine(projectItem.Project.DirectoryPath, Include));
      File = new FileInfoWrap(fullPath);
      Project = solution.GetProjectByAbsoluteProjectFilePath(fullPath);

      Guid referencedProjectGuid;
      if (Guid.TryParse(projectItem.GetMetadataValue("Project"), out referencedProjectGuid))
        ReferencedProjectGuid = referencedProjectGuid;

      ReferencedProjectName = projectItem.GetMetadataValue("Name");
    }
  }
}