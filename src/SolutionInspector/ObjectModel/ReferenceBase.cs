using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
  internal abstract class ReferenceBase : IReference
  {
    public AssemblyName AssemblyName { get; }

    protected ReferenceBase (AssemblyName assemblyName)
    {
      AssemblyName = assemblyName;
    }
  }
}