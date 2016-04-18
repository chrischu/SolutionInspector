using System.Reflection;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Represents a DLL reference via the file system.
  /// </summary>
  public class FileReference : DllReferenceBase
  {
    /// <summary>
    /// The hint path that points to the DLL in the file system.
    /// </summary>
    public string HintPath { get; }

    /// <summary>
    /// Creates a new <see cref="FileReference"/>.
    /// </summary>
    public FileReference(AssemblyName assemblyName, string hintPath)
        : base(assemblyName)
    {
      HintPath = hintPath;
    }
  }
}