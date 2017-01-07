using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Build.Evaluation;
using SolutionInspector.Api.Rules;
using Wrapperator.Interfaces.IO;

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
    ProjectItem OriginalProjectItem { get; set; }


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
    ///   True, if the file was referenced with wildcards (see <see cref="WildcardInclude" />) as opposed to a direct file name reference.
    /// </summary>
    bool IsIncludedByWildcard { get; }

    /// <summary>
    ///   The include path(s) (including a wildcard) for the project item or <see langword="null" /> if it wasn't included via wildcard.
    /// </summary>
    string WildcardInclude { get; }

    /// <summary>
    ///   The exclude path(s) (including a wildcard) for the project item or <see langword="null" /> if it wasn't included via wildcard.
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
}