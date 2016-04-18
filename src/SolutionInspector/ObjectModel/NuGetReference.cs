using System.Reflection;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a DLL reference via NuGet.
  /// </summary>
  public class NuGetReference : DllReferenceBase
  {
    /// <summary>
    /// The NuGet package that created the reference.
    /// </summary>
    public NuGetPackage Package { get; }

    /// <summary>
    /// <c>True</c> if the reference is private, <c>false</c> otherwise.
    /// </summary>
    public bool IsPrivate { get; }

    /// <summary>
    /// The hint path that points to the DLL in the NuGet packages folder.
    /// </summary>
    public string HintPath { get; }

    /// <summary>
    /// Creates a new <see cref="NuGetReference"/>.
    /// </summary>
    public NuGetReference(NuGetPackage package, AssemblyName assemblyName, bool isPrivate, string hintPath)
        : base(assemblyName)
    {
      Package = package;
      IsPrivate = isPrivate;
      HintPath = hintPath;
    }
  }
}