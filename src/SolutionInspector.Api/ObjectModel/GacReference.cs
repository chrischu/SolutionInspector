using System;
using System.Reflection;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  /// Represents a DLL reference via the GAC (Global Assembly Cache).
  /// </summary>
  public class GacReference : DllReferenceBase
  {
    /// <summary>
    /// Creates a new <see cref="GacReference"/>.
    /// </summary>
    public GacReference(AssemblyName assemblyName)
        : base(assemblyName)
    {
    }
  }
}