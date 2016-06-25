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
    NuGetPackage Package { get; }

    /// <summary>
    ///   <c>True</c> if the reference is private, <c>false</c> otherwise.
    /// </summary>
    bool IsPrivate { get; }

    /// <summary>
    ///   The hint path that points to the DLL in the NuGet packages folder.
    /// </summary>
    string HintPath { get; }
  }

  internal class NuGetReference : DllReferenceBase, INuGetReference
  {
    public NuGetPackage Package { get; }

    public bool IsPrivate { get; }

    public string HintPath { get; }

    public NuGetReference (NuGetPackage package, AssemblyName assemblyName, bool isPrivate, string hintPath, string projectDirectory)
        : base(assemblyName, projectDirectory, hintPath)
    {
      Package = package;
      IsPrivate = isPrivate;
      HintPath = hintPath;
    }
  }
}