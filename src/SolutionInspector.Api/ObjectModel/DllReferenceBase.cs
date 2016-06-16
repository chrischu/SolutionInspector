using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Base class that represents a project's DLL reference.
  /// </summary>
  [PublicAPI]
  public interface IDllReference
  {
    /// <summary>
    ///   The <see cref="AssemblyName" /> of the referenced DLL.
    /// </summary>
    AssemblyName AssemblyName { get; }
  }

  internal abstract class DllReferenceBase : IDllReference
  {
    public AssemblyName AssemblyName { get; }

    protected DllReferenceBase (AssemblyName assemblyName)
    {
      AssemblyName = assemblyName;
    }
  }
}