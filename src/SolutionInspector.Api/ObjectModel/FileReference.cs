using System;
using System.Reflection;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a DLL reference via the file system.
  /// </summary>
  public interface IFileReference : IDllReference
  {
    /// <summary>
    ///   The hint path that points to the DLL in the file system.
    /// </summary>
    string HintPath { get; }
  }

  internal class FileReference : DllReferenceBase, IFileReference
  {
    public string HintPath { get; }

    public FileReference (AssemblyName assemblyName, string hintPath)
        : base(assemblyName)
    {
      HintPath = hintPath;
    }
  }
}