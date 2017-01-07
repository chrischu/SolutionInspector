using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers;

namespace SolutionInspector.ObjectModel
{
  internal class ProjectItem : IProjectItem
  {
    private readonly List<ProjectItem> _children = new List<ProjectItem>();
    private Lazy<string> _identifier;

    protected ProjectItem (
      IProject project,
      Microsoft.Build.Evaluation.ProjectItem msBuildProjectItem)
    {
      Project = project;
      OriginalProjectItem = msBuildProjectItem;

      Include = new ProjectItemInclude(msBuildProjectItem.EvaluatedInclude, msBuildProjectItem.UnevaluatedInclude);

      BuildAction = ProjectItemBuildAction.Custom(msBuildProjectItem.ItemType);
      var fullPath = Path.GetFullPath(Path.Combine(project.ProjectFile.DirectoryName, msBuildProjectItem.EvaluatedInclude));
      File = Wrapper.Wrap(new FileInfo(fullPath));

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