using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.Configuration.Dynamic
{
  /// <summary>
  ///   Thrown when a dynamic element type compatibility validation fails.
  /// </summary>
  [Serializable]
  public class DynamicElementTypeCompatibilityException : Exception
  {
    public DynamicElementTypeCompatibilityException (string message)
        : base(message)
    {
    }

    [ExcludeFromCodeCoverage]
    protected DynamicElementTypeCompatibilityException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}