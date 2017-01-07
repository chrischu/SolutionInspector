using System;
using System.Reflection;
using JetBrains.Annotations;
using SolutionInspector.Api.Rules;

namespace SolutionInspector.Internals
{
  /// <summary>
  ///   Captures information about the type of a <see cref="Rule{TTarget}" />.
  /// </summary>
  public class RuleTypeInfo
  {
    /// <summary>
    ///   Constructor.
    /// </summary>
    public RuleTypeInfo (Type ruleType, [CanBeNull] Type configurationType, ConstructorInfo constructor)
    {
      RuleType = ruleType;
      Constructor = constructor;
      ConfigurationType = configurationType;
    }

    /// <summary>
    ///   The CLR type of the rule.
    /// </summary>
    public Type RuleType { get; }

    /// <summary>
    ///   The CLR type of the configuration of the rule (if any).
    /// </summary>
    [CanBeNull]
    public Type ConfigurationType { get; }

    /// <summary>
    ///   Returns <see langword="true" /> when the rule is configurable, <see langword="false" /> otherwise.
    /// </summary>
    public bool IsConfigurable => ConfigurationType != null;

    /// <summary>
    ///   The constructor of the rule.
    /// </summary>
    public ConstructorInfo Constructor { get; }
  }
}