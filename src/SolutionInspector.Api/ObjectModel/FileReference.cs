using System;
using System.Reflection;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a DLL reference via the file system.
  /// </summary>
  public interface IFileReference : IDllReference
  {
  }

  internal class FileReference : DllReferenceBase, IFileReference
  {
    public FileReference (AssemblyName assemblyName, string hintPath, string projectDirectory)
        : base(assemblyName, projectDirectory, hintPath)
    {
    }
  }
}