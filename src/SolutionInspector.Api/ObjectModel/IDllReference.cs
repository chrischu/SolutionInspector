using System;
using SolutionInspector.Commons.Attributes;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a reference pointing to a DLL in the file system.
  /// </summary>
  [PublicApi]
  public interface IDllReference : IReference
  {
    /// <summary>
    ///   The referenced DLL file.
    /// </summary>
    IFileInfo DllFile { get; }

    /// <summary>
    ///   The hint path that points to the DLL in the file system.
    /// </summary>
    string HintPath { get; }
  }
}