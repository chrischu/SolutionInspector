using System;
using System.Xml;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a NuGet package referenced by a project.
  /// </summary>
  [PublicAPI]
  public class NuGetPackage : IEquatable<NuGetPackage>
  {
    /// <summary>
    ///   The package's id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///   The package's version.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    ///   <c>True</c> if the package is a pre-release package, <c>false</c> otherwise.
    /// </summary>
    public bool IsPreRelease { get; }

    /// <summary>
    ///   The package's pre-release tag (if any).
    /// </summary>
    public string PreReleaseTag { get; }

    /// <summary>
    ///   The package's full version string in the format "&lt;Version&gt;&lt;PreReleaseTag&gt;".
    /// </summary>
    public string FullVersionString => $"{Version}{PreReleaseTag}";

    /// <summary>
    ///   The package's directory name (relative to the NuGet packages folder)".
    /// </summary>
    public string PackageDirectoryName => $@"{Id}.{FullVersionString}";

    /// <summary>
    ///   The package's target framework.
    /// </summary>
    public string TargetFramework { get; }

    /// <summary>
    ///   <c>True</c> if the package is only a development dependency, <c>false</c> otherwise.
    /// </summary>
    public bool IsDevelopmentDependency { get; }

    /// <summary>
    ///   Creates a new <see cref="NuGetPackage" />.
    /// </summary>
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

    /// <summary>
    ///   Creates a <see cref="NuGetPackage" /> instance from the given <paramref name="packageElement" /> from the project packages.config.
    /// </summary>
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

      var isDevelopmentDependency = packageElement.HasAttribute("targetElement") && packageElement.GetAttribute("targetElement") == "true";

      return new NuGetPackage(id, version, isPreRelease, preReleaseTag, targetFramework, isDevelopmentDependency);
    }

    /// <inheritdoc />
    public bool Equals ([CanBeNull] NuGetPackage other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(PackageDirectoryName, other.PackageDirectoryName);
    }

    /// <inheritdoc />
    public override bool Equals ([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((NuGetPackage) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode ()
    {
      return PackageDirectoryName.GetHashCode();
    }

    /// <inheritdoc />
    public static bool operator == ([CanBeNull] NuGetPackage left, [CanBeNull] NuGetPackage right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
    public static bool operator != ([CanBeNull] NuGetPackage left, [CanBeNull] NuGetPackage right)
    {
      return !Equals(left, right);
    }
  }
}