using System;
using System.Reflection;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a DLL reference via NuGet.
  /// </summary>
  public interface INuGetReference : IDllReference
  {
    /// <summary>
    ///   The NuGet package that created the reference.
    /// </summary>
    INuGetPackage Package { get; }

    /// <summary>
    ///   <c>True</c> if the reference is private, <c>false</c> otherwise.
    /// </summary>
    bool IsPrivate { get; }
  }

  internal class NuGetReference : DllReferenceBase, INuGetReference
  {
    public INuGetPackage Package { get; }

    public bool IsPrivate { get; }

    public NuGetReference (INuGetPackage package, AssemblyName assemblyName, bool isPrivate, string hintPath, string projectDirectory)
        : base(assemblyName, projectDirectory, hintPath)
    {
      Package = package;
      IsPrivate = isPrivate;
    }
  }
}