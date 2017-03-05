using System.IO;
using System.Reflection;
using SolutionInspector.Api.ObjectModel;
using Wrapperator.Interfaces.IO;
using Wrapperator.Wrappers;

namespace SolutionInspector.Internals.ObjectModel
{
  internal abstract class DllReferenceBase : ReferenceBase, IDllReference
  {
    protected DllReferenceBase (AssemblyName assemblyName, string projectDirectory, string hintPath)
      : base(assemblyName)
    {
      HintPath = hintPath;
      DllFile = Wrapper.Wrap(new FileInfo(Path.GetFullPath(Path.Combine(projectDirectory, hintPath))));
    }

    public IFileInfo DllFile { get; }

    public string HintPath { get; }
  }
}