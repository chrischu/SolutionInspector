using System;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a NuGet package referenced by a project.
  /// </summary>
  [PublicAPI]
  public interface INuGetPackage : IEquatable<INuGetPackage>
  {
    /// <summary>
    ///   The package's id.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///   The package's version.
    /// </summary>
    Version Version { get; }

    /// <summary>
    ///   <c>True</c> if the package is a pre-release package, <c>false</c> otherwise.
    /// </summary>
    bool IsPreRelease { get; }

    /// <summary>
    ///   The package's pre-release tag (if any).
    /// </summary>
    string PreReleaseTag { get; }

    /// <summary>
    ///   The package's full version string in the format "&lt;Version&gt;&lt;PreReleaseTag&gt;".
    /// </summary>
    string FullVersionString { get; }

    /// <summary>
    ///   The package's directory name (relative to the NuGet packages folder)".
    /// </summary>
    string PackageDirectoryName { get; }

    /// <summary>
    ///   The package's target framework.
    /// </summary>
    string TargetFramework { get; }

    /// <summary>
    ///   <c>True</c> if the package is only a development dependency, <c>false</c> otherwise.
    /// </summary>
    bool IsDevelopmentDependency { get; }
  }
}