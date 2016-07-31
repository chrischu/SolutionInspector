using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a reference pointing to a DLL in the file system.
  /// </summary>
  [PublicAPI]
  public interface IDllReference : IReference
  {
    /// <summary>
    ///   The referenced DLL file.
    /// </summary>
    IFileInfo DllFile { get; }

    /// <summary>
    ///   The hint path that points to the DLL in the file system.
    /// </summary>
    string HintPath { get; }
  }

  internal abstract class DllReferenceBase : ReferenceBase, IDllReference
  {
    public IFileInfo DllFile { get; }

    public string HintPath { get; }

    protected DllReferenceBase (AssemblyName assemblyName, string projectDirectory, string hintPath)
        : base(assemblyName)
    {
      HintPath = hintPath;
      DllFile = Wrapper.Wrap(new FileInfo(Path.GetFullPath(Path.Combine(projectDirectory, hintPath))));
    }
  }
}