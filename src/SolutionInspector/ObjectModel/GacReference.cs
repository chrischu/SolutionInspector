using System.Reflection;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.ObjectModel
{
  internal class GacReference : ReferenceBase, IGacReference
  {
    public GacReference (AssemblyName assemblyName)
        : base(assemblyName)
    {
    }
  }
}