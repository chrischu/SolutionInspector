using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Internals.ObjectModel
{
  internal abstract class ReferenceBase : IReference
  {
    protected ReferenceBase (AssemblyName assemblyName)
    {
      AssemblyName = assemblyName;
    }

    public AssemblyName AssemblyName { get; }
  }
}