using System;
using System.Runtime.Serialization;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  /// Represents errors that occurs when trying to load a rule assembly that does not exist.
  /// </summary>
  [Serializable]
  public class RuleAssemblyNotFoundException : Exception
  {
    /// <summary>
    /// Creates a new <see cref="RuleAssemblyNotFoundException"/>
    /// </summary>
    public RuleAssemblyNotFoundException(string ruleAssemblyPath, Exception innerException = null)
        : base($"Could not find rule assembly '{ruleAssemblyPath}'.", innerException)
    {
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    protected RuleAssemblyNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}