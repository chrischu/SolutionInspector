using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;
using JetBrains.Annotations;
using SolutionInspector.Extensions;
using SolutionInspector.Rules;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a MSBuild project item.
  /// </summary>
  [PublicAPI]
  public interface IProjectItem : IRuleTarget
  {
    /// <summary>
    /// The project the item is contained in.
    /// </summary>
    IProject Project { get; }

    /// <summary>
    /// The (file) name of the project item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The path (relative to the project file) of the project item.
    /// </summary>
    string Include { get; }

    /// <summary>
    /// The build action that is configured for the project item.
    /// </summary>
    ProjectItemBuildAction BuildAction { get; }

    /// <summary>
    /// A <see cref="IFileInfo"/> pointing to the project item.
    /// </summary>
    IFileInfo File { get; }

    /// <summary>
    /// Metadata for the project item.
    /// </summary>
    IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>
    /// The custom tool that is configured for the project item.
    /// </summary>
    string CustomTool { get; }

    /// <summary>
    /// The custom tool namespace that is configured for the project item.
    /// </summary>
    string CustomToolNamespace { get; }

    /// <summary>
    /// The project item's parent project item (if any).
    /// </summary>
    IProjectItem Parent { get; }

    /// <summary>
    /// The project item's child project items (if any).
    /// </summary>
    IReadOnlyCollection<IProjectItem> Children { get; }
  }

  [PublicAPI]
  internal class ProjectItem : IProjectItem
  {
    private readonly List<ProjectItem> _children = new List<ProjectItem>();

    public IProject Project { get; }

    public string Name => Path.GetFileName(Include);
    public string Identifier => Name;
    public string FullPath => File.FullName;

    public string Include { get; }

    public ProjectItemBuildAction BuildAction { get; }

    public IFileInfo File { get; }

    public IReadOnlyDictionary<string, string> Metadata { get; }

    public string CustomTool => Metadata.GetValueOrDefault("Generator");
    public string CustomToolNamespace => Metadata.GetValueOrDefault("CustomToolNamespace");


    public IProjectItem Parent { get; private set; }

    public IReadOnlyCollection<IProjectItem> Children => _children;

    public ProjectItem(IProject project, string include, ProjectItemBuildAction buildAction, IFileInfo file, IReadOnlyDictionary<string, string> metadata)
    {
      Project = project;
      Include = include;
      BuildAction = buildAction;
      File = file;
      Metadata = metadata;
    }

    internal void SetParent(ProjectItem parent)
    {
      parent._children.Add(this);
      Parent = parent;
    }

    public static ProjectItem FromMsBuildProjectItem(IProject project, Microsoft.Build.Evaluation.ProjectItem msBuildProjectItem)
    {
      var buildAction = ProjectItemBuildAction.Custom(msBuildProjectItem.ItemType);

      var fullPath = Path.GetFullPath(Path.Combine(project.ProjectFile.DirectoryName, msBuildProjectItem.EvaluatedInclude));
      var file = new FileInfoWrap(fullPath);
      var metadata = msBuildProjectItem.Metadata.ToDictionary(m => m.Name, m => m.EvaluatedValue);

      return new ProjectItem(project, msBuildProjectItem.EvaluatedInclude, buildAction, file, metadata);
    }

  }
}