using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   Represents errors that occurs when trying to instantiate a <see cref="IRule" />.
  /// </summary>
  [Serializable]
  public class RuleInstantiationException : Exception
  {
    /// <summary>
    ///   Creates a new <see cref="RuleInstantiationException" />
    /// </summary>
    public RuleInstantiationException (string message, Exception innerException = null)
      : base(message, innerException)
    {
    }

    /// <summary>
    ///   Serialization constructor.
    /// </summary>
    [ExcludeFromCodeCoverage /* Serialization ctor */]
    protected RuleInstantiationException (SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}