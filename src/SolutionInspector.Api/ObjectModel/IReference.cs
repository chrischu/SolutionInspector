using System.Reflection;
using JetBrains.Annotations;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a reference from a <see cref="IProject" />.
  /// </summary>
  [PublicAPI]
  public interface IReference
  {
    /// <summary>
    ///   The <see cref="AssemblyName" /> of the referenced DLL.
    /// </summary>
    AssemblyName AssemblyName { get; }
  }
}