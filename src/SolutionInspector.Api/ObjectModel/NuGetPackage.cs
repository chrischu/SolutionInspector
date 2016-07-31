using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
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

  internal sealed class NuGetPackage : INuGetPackage
  {
    public string Id { get; }

    public Version Version { get; }

    public bool IsPreRelease { get; }

    public string PreReleaseTag { get; }

    public string FullVersionString => $"{Version}{PreReleaseTag}";

    public string PackageDirectoryName => $"{Id}.{FullVersionString}";

    public string TargetFramework { get; }

    public bool IsDevelopmentDependency { get; }

    public NuGetPackage (
        string id,
        Version version,
        bool isPreRelease,
        [CanBeNull] string preReleaseTag,
        string targetFramework,
        bool isDevelopmentDependency)
    {
      Id = id;
      Version = version;
      IsPreRelease = isPreRelease;
      PreReleaseTag = preReleaseTag;
      TargetFramework = targetFramework;
      IsDevelopmentDependency = isDevelopmentDependency;
    }

    public static NuGetPackage FromXmlElement (XmlElement packageElement)
    {
      var id = packageElement.GetAttribute("id");
      bool isPreRelease = false;
      string preReleaseTag = null;

      var versionString = packageElement.GetAttribute("version");
      if (versionString.Contains("-"))
      {
        var split = versionString.Split('-');
        isPreRelease = true;
        preReleaseTag = "-" + split[1];
        versionString = split[0];
      }

      var version = Version.Parse(versionString);

      var targetFramework = packageElement.GetAttribute("targetFramework");

      var isDevelopmentDependency = packageElement.HasAttribute("developmentDependency")
                                    && packageElement.GetAttribute("developmentDependency") == "true";

      return new NuGetPackage(id, version, isPreRelease, preReleaseTag, targetFramework, isDevelopmentDependency);
    }

    public bool Equals ([CanBeNull] INuGetPackage other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(PackageDirectoryName, other.PackageDirectoryName);
    }

    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is INuGetPackage && Equals((INuGetPackage) obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode ()
    {
      return PackageDirectoryName.GetHashCode();
    }

    public static bool operator == ([CanBeNull] NuGetPackage left, [CanBeNull] NuGetPackage right)
    {
      return Equals(left, right);
    }

    public static bool operator != ([CanBeNull] NuGetPackage left, [CanBeNull] NuGetPackage right)
    {
      return !Equals(left, right);
    }
  }
}