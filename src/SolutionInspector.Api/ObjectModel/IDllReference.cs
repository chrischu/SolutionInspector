using System;
using JetBrains.Annotations;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Api.ObjectModel
{
  /// <summary>
  ///   Represents a reference pointing to a DLL in the file system.
  /// </summary>
  [PublicAPI]
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