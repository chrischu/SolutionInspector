using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.Api.Utilities
{
  /// <summary>
  ///   Represents errors that occurs when trying to load a solution file that does not exist.
  /// </summary>
  [Serializable]
  public class SolutionNotFoundException : Exception
  {
    /// <summary>
    ///   Creates a new <see cref="SolutionNotFoundException" />
    /// </summary>
    public SolutionNotFoundException (string solutionPath, Exception innerException = null)
        : base($"Could not find solution file at '{solutionPath}'.", innerException)
    {
    }

    /// <summary>
    ///   Serialization constructor.
    /// </summary>
    [ExcludeFromCodeCoverage /* Serialization ctor */]
    protected SolutionNotFoundException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}