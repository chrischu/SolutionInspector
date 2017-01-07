using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
  internal class FileReference : DllReferenceBase, IFileReference
  {
    public FileReference (AssemblyName assemblyName, string hintPath, string projectDirectory)
      : base(assemblyName, projectDirectory, hintPath)
    {
    }
  }
}