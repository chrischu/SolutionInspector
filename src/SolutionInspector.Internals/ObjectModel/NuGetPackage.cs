using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Internals.ObjectModel
{
  /// <inheritdoc cref="INuGetPackage" />
  public sealed class NuGetPackage : INuGetPackage
  {
    internal NuGetPackage (
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

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public Version Version { get; }

    /// <inheritdoc />
    public bool IsPreRelease { get; }

    /// <inheritdoc />
    public string PreReleaseTag { get; }

    /// <inheritdoc />
    public string FullVersionString => $"{Version}{PreReleaseTag}";

    /// <inheritdoc />
    public string PackageDirectoryName => $"{Id}.{FullVersionString}";

    /// <inheritdoc />
    public string TargetFramework { get; }

    /// <inheritdoc />
    public bool IsDevelopmentDependency { get; }

    /// <inheritdoc />
    public bool Equals([CanBeNull] INuGetPackage other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return string.Equals(PackageDirectoryName, other.PackageDirectoryName);
    }

    internal static NuGetPackage FromXmlElement (XmlElement packageElement)
    {
      var id = packageElement.GetAttribute("id");
      var isPreRelease = false;
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

    /// <inheritdoc />
    public override bool Equals([CanBeNull] object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return obj is INuGetPackage && Equals((INuGetPackage) obj);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
      return PackageDirectoryName.GetHashCode();
    }

    /// <summary>
    ///   Compares two <see cref="NuGetPackage" />s by using <see cref="Equals(object)" /> and returns <see langword="true" /> if
    ///   they are equal, <see langword="false" /> otherwise.
    /// </summary>
    public static bool operator == ([CanBeNull] NuGetPackage left, [CanBeNull] NuGetPackage right)
    {
      return Equals(left, right);
    }

    /// <summary>
    ///   Compares two <see cref="NuGetPackage" />s by using <see cref="Equals(object)" /> and returns <see langword="false" /> if
    ///   they are equal, <see langword="true" /> otherwise.
    /// </summary>
    public static bool operator != ([CanBeNull] NuGetPackage left, [CanBeNull] NuGetPackage right)
    {
      return !Equals(left, right);
    }
  }
}