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
    IProjectItemInclude Include { get; }

    /// <summary>
    ///   The build action that is configured for the project item.
    /// </summary>
    ProjectItemBuildAction BuildAction { get; }

    /// <summary>
    ///   A <see cref="IFileInfo" /> pointing to the project item.
    /// </summary>
    IFileInfo File { get; }

    /// <summary>
    ///   True, if the file is referenced as a link instead of a regular reference.
    /// </summary>
    /// <remarks>
    ///   Links are usually used to reference one file in multiple projects while still maintaining only one physical copy of it.
    /// </remarks>
    bool IsLink { get; }

    /// <summary>
    ///   True, if the file was referenced with wildcards (see <see cref="WildcardInclude"/>) as opposed to a direct file name reference.
    /// </summary>
    bool IsIncludedByWildcard { get; }

    /// <summary>
    /// The include path(s) (including a wildcard) for the project item or <see langword="null" /> if it wasn't included via wildcard.
    /// </summary>
    string WildcardInclude { get; }

    /// <summary>
    /// The exclude path(s) (including a wildcard) for the project item or <see langword="null" /> if it wasn't included via wildcard.
    /// </summary>
    string WildcardExclude { get; }

    /// <summary>
    ///   The project item's location inside the project file.
    /// </summary>
    IProjectLocation Location { get; }

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

    public string Name => Path.GetFileName(Include.Evaluated);

    public string Identifier => _identifier.Value;

    public string FullPath => File.FullName;

    public IProjectItemInclude Include { get; }

    public ProjectItemBuildAction BuildAction { get; }

    public IFileInfo File { get; }
    public bool IsLink { get; }
    public bool IsIncludedByWildcard { get; }
    public string WildcardInclude { get; }
    public string WildcardExclude { get; }

    public IProjectLocation Location { get; }

    public IReadOnlyDictionary<string, string> Metadata { get; }

    public string CustomTool => Metadata.GetValueOrDefault("Generator");
    public string CustomToolNamespace => Metadata.GetValueOrDefault("CustomToolNamespace");


    public IProjectItem Parent { get; private set; }

    public IReadOnlyCollection<IProjectItem> Children => _children;

    protected ProjectItem (
        IProject project,
        Microsoft.Build.Evaluation.ProjectItem msBuildProjectItem)
    {
      Project = project;
      OriginalProjectItem = msBuildProjectItem;

      Include = new ProjectItemInclude(msBuildProjectItem.EvaluatedInclude, msBuildProjectItem.UnevaluatedInclude);

      BuildAction = ProjectItemBuildAction.Custom(msBuildProjectItem.ItemType);
      var fullPath = Path.GetFullPath(Path.Combine(project.ProjectFile.DirectoryName, msBuildProjectItem.EvaluatedInclude));
      File = new FileInfoWrap(fullPath);

      Location = new ProjectLocation(msBuildProjectItem.Xml.Location.Line, msBuildProjectItem.Xml.Location.Column);
      Metadata = msBuildProjectItem.Metadata.ToDictionary(m => m.Name, m => m.EvaluatedValue);
      IsLink = Metadata.ContainsKey("Link");

      if (msBuildProjectItem.UnevaluatedInclude.Contains("*"))
      {
        IsIncludedByWildcard = true;
        WildcardInclude = msBuildProjectItem.UnevaluatedInclude;
        WildcardExclude = msBuildProjectItem.Xml.Exclude;
      }

      _identifier = new Lazy<string>(CreateIdentifier);
    }

    private string CreateIdentifier ()
    {
      var sb = new StringBuilder();

      sb.Append(Parent != null ? Parent.Identifier : Project.Identifier);
      sb.Append('/');

      var include = IsLink ? Metadata["Link"] : Include.Evaluated;

      include = include.Replace('\\', '/');
      if (Parent != null)
        include = include.Split('/').Last();

      sb.Append(include);

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