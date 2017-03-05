using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Internals.ObjectModel
{
  internal class FileReference : DllReferenceBase, IFileReference
  {
    public FileReference (AssemblyName assemblyName, string hintPath, string projectDirectory)
      : base(assemblyName, projectDirectory, hintPath)
    {
    }
  }
}