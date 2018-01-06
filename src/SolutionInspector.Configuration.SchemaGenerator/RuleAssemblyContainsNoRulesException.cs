using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.SchemaGenerator
{
  /// <summary>
  ///   Thrown when a rule assembly contains no rules.
  /// </summary>
  [Serializable]
  public class RuleAssemblyContainsNoRulesException : Exception
  {
    public RuleAssemblyContainsNoRulesException (string message)
        : base(message)
    {
    }

    [ExcludeFromCodeCoverage]
    protected RuleAssemblyContainsNoRulesException (SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}