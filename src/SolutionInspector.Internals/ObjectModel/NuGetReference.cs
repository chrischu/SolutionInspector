using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Internals.ObjectModel
{
  internal class NuGetReference : DllReferenceBase, INuGetReference
  {
    public NuGetReference (INuGetPackage package, AssemblyName assemblyName, bool isPrivate, string hintPath, string projectDirectory)
      : base(assemblyName, projectDirectory, hintPath)
    {
      Package = package;
      IsPrivate = isPrivate;
    }

    public INuGetPackage Package { get; }

    public bool IsPrivate { get; }
  }
}