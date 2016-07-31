using System;
using System.Reflection;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a DLL reference via the GAC (Global Assembly Cache).
  /// </summary>
  public interface IGacReference : IReference
  {
  }

  internal class GacReference : ReferenceBase, IGacReference
  {
    public GacReference (AssemblyName assemblyName)
        : base(assemblyName)
    {
    }
  }
}