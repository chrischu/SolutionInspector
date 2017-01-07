using System;
using System.IO;
using SolutionInspector.Api.ObjectModel;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers;

namespace SolutionInspector.ObjectModel
{
  internal class ProjectReference : IProjectReference
  {
    public ProjectReference (ISolution solution, Microsoft.Build.Evaluation.ProjectItem projectItem)
    {
      OriginalProjectItem = projectItem;
      Include = projectItem.EvaluatedInclude;

      var fullPath = Path.GetFullPath(Path.Combine(projectItem.Project.DirectoryPath, Include));
      File = Wrapper.Wrap(new FileInfo(fullPath));
      Project = solution.GetProjectByAbsoluteProjectFilePath(fullPath);

      Guid referencedProjectGuid;
      if (Guid.TryParse(projectItem.GetMetadataValue("Project"), out referencedProjectGuid))
        ReferencedProjectGuid = referencedProjectGuid;

      ReferencedProjectName = projectItem.GetMetadataValue("Name");
    }

    public IProject Project { get; }
    public Microsoft.Build.Evaluation.ProjectItem OriginalProjectItem { get; }
    public string Include { get; }
    public IFileInfo File { get; }
    public Guid? ReferencedProjectGuid { get; }
    public string ReferencedProjectName { get; }
  }
}