using System;
using System.Runtime.Serialization;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   Represents errors that occurs when trying to resolve the type of a <see cref="IRule" />.
  /// </summary>
  [Serializable]
  public class RuleTypeResolvingException : Exception
  {
    /// <summary>
    ///   Creates a new <see cref="RuleTypeResolvingException" />
    /// </summary>
    public RuleTypeResolvingException (string message, Exception innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    ///   Serialization constructor.
    /// </summary>
    protected RuleTypeResolvingException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}