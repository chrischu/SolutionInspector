using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.ObjectModel
{
  /// <summary>
  /// Base class that represents a project's DLL reference.
  /// </summary>
  [PublicAPI]
  public abstract class DllReferenceBase
  {
    /// <summary>
    /// The <see cref="AssemblyName"/> of the referenced DLL.
    /// </summary>
    public AssemblyName AssemblyName { get; }

    /// <summary>
    /// Creates a new DLL reference.
    /// </summary>
    protected DllReferenceBase(AssemblyName assemblyName)
    {
      AssemblyName = assemblyName;
    }
  }
}