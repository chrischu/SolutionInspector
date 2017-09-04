using System.Reflection;
using SolutionInspector.Commons.Attributes;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a reference from a <see cref="IProject" />.
  /// </summary>
  [PublicApi]
  public interface IReference
  {
    /// <summary>
    ///   The <see cref="AssemblyName" /> of the referenced DLL.
    /// </summary>
    AssemblyName AssemblyName { get; }
  }
}