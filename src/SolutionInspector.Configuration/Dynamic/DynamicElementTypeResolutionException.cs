using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.Configuration.Dynamic
{
  /// <summary>
  ///   Thrown when dynamic element type resolution fails.
  /// </summary>
  [Serializable]
  public class DynamicElementTypeResolutionException : Exception
  {
    public DynamicElementTypeResolutionException (string message)
        : base(message)
    {
    }

    public DynamicElementTypeResolutionException (string message, Exception innerException)
        : base(message, innerException)
    {
    }

    [ExcludeFromCodeCoverage]
    protected DynamicElementTypeResolutionException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}