using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   Represents errors that occurs when trying to load a rule assembly that does not contain rules.
  /// </summary>
  [Serializable]
  public class InvalidRuleAssemblyException : Exception
  {
    /// <summary>
    ///   Creates a new <see cref="InvalidRuleAssemblyException" />
    /// </summary>
    public InvalidRuleAssemblyException (string ruleAssemblyPath, Exception innerException = null)
      : base($"The assembly loaded from '{ruleAssemblyPath}' is not a valid rule assembly.", innerException)
    {
    }

    /// <summary>
    ///   Serialization constructor.
    /// </summary>
    [ExcludeFromCodeCoverage /* Serialization ctor */]
    protected InvalidRuleAssemblyException (SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}