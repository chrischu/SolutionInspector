using System;
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

    public DynamicElementTypeCompatibilityException (string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected DynamicElementTypeCompatibilityException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}