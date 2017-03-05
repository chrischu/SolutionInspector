using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Internals.ObjectModel
{
  internal class GacReference : ReferenceBase, IGacReference
  {
    public GacReference (AssemblyName assemblyName)
      : base(assemblyName)
    {
    }
  }
}