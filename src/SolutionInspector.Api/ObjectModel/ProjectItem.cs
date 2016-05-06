using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SystemInterface.IO;
using SystemWrapper.IO;
using JetBrains.Annotations;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a MSBuild project item.
  /// </summary>
  [PublicAPI]
  public interface IProjectItem : IRuleTarget
  {
    /// <summary>
    ///   The original MSBuild project item.
    /// </summary>
    Microsoft.Build.Evaluation.ProjectItem OriginalProjectItem { get; set; }


    /// <summary>
    ///   The project the item is contained in.
    /// </summary>
    IProject Project { get; }

    /// <summary>
    ///   The (file) name of the project item.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The original (just as in the MSBuild file) include path (relative to the project file) of the project item.
    /// </summary>
    string OriginalInclude { get; }

    /// <summary>
    ///   The path (relative to the project file) of the project item.
    ///   Differs from <see cref="OriginalInclude" /> for links.
    /// </summary>
    string Include { get; }

    /// <summary>
    ///   The build action that is configured for the project item.
    /// </summary>
    ProjectItemBuildAction BuildAction { get; }

    /// <summary>
    ///   A <see cref="IFileInfo" /> pointing to the project item.
    /// </summary>
    IFileInfo File { get; }

    /// <summary>
    ///   Metadata for the project item.
    /// </summary>
    IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>
    ///   The custom tool that is configured for the project item.
    /// </summary>
    string CustomTool { get; }

    /// <summary>
    ///   The custom tool namespace that is configured for the project item.
    /// </summary>
    string CustomToolNamespace { get; }

    /// <summary>
    ///   The project item's parent project item (if any).
    /// </summary>
    IProjectItem Parent { get; }

    /// <summary>
    ///   The project item's child project items (if any).
    /// </summary>
    IReadOnlyCollection<IProjectItem> Children { get; }
  }

  [PublicAPI]
  internal class ProjectItem : IProjectItem
  {
    private readonly List<ProjectItem> _children = new List<ProjectItem>();
    private Lazy<string> _identifier;

    public Microsoft.Build.Evaluation.ProjectItem OriginalProjectItem { get; set; }

    public IProject Project { get; }

    public string Name => Path.GetFileName(OriginalInclude);

    public string Identifier => _identifier.Value;

    public string FullPath => File.FullName;

    public string OriginalInclude { get; }
    public string Include { get; }

    public ProjectItemBuildAction BuildAction { get; }

    public IFileInfo File { get; }

    public IReadOnlyDictionary<string, string> Metadata { get; }

    public string CustomTool => Metadata.GetValueOrDefault("Generator");
    public string CustomToolNamespace => Metadata.GetValueOrDefault("CustomToolNamespace");


    public IProjectItem Parent { get; private set; }

    public IReadOnlyCollection<IProjectItem> Children => _children;

    public ProjectItem (
        IProject project,
        Microsoft.Build.Evaluation.ProjectItem msBuildProjectItem)
    {
      Project = project;
      OriginalInclude = msBuildProjectItem.EvaluatedInclude;
      OriginalProjectItem = msBuildProjectItem;

      var linkMetadata = msBuildProjectItem.DirectMetadata.SingleOrDefault(d => d.Name == "Link");
      Include = linkMetadata?.EvaluatedValue ?? OriginalInclude;

      BuildAction = ProjectItemBuildAction.Custom(msBuildProjectItem.ItemType);
      var fullPath = Path.GetFullPath(Path.Combine(project.ProjectFile.DirectoryName, msBuildProjectItem.EvaluatedInclude));
      File = new FileInfoWrap(fullPath);
      Metadata = msBuildProjectItem.Metadata.ToDictionary(m => m.Name, m => m.EvaluatedValue);

      _identifier = new Lazy<string>(CreateIdentifier);
    }

    private string CreateIdentifier ()
    {
      var sb = new StringBuilder(Project.Identifier);
      sb.Append('/');

      if (Parent != null)
      {
        sb.Append(Parent.Identifier);
        sb.Append('/');
      }

      sb.Append(Include.Replace('\\', '/'));

      return sb.ToString();
    }

    internal void SetParent (ProjectItem parent)
    {
      parent._children.Add(this);
      Parent = parent;
    }

    public static ProjectItem FromMsBuildProjectItem (IProject project, Microsoft.Build.Evaluation.ProjectItem msBuildProjectItem)
    {
      return new ProjectItem(project, msBuildProjectItem);
    }
  }
}