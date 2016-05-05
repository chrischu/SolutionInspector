using System;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A violation of a <see cref="IRule" />.
  /// </summary>
  public interface IRuleViolation
  {
    /// <summary>
    ///   The <see cref="IRule" /> that was violated.
    /// </summary>
    IRule Rule { get; }

    /// <summary>
    ///   The <see cref="IRuleTarget" /> that caused the <see cref="IRuleViolation" />.
    /// </summary>
    IRuleTarget Target { get; }

    /// <summary>
    ///   A message that describes the nature of the <see cref="IRuleViolation" />.
    /// </summary>
    string Message { get; }
  }

  /// <inheritdoc />
  [PublicAPI]
  public class RuleViolation : IRuleViolation
  {
    /// <inheritdoc />
    public IRule Rule { get; }

    /// <inheritdoc />
    public IRuleTarget Target { get; set; }

    /// <inheritdoc />
    public string Message { get; }

    /// <summary>
    ///   Creates a new <see cref="RuleViolation" />.
    /// </summary>
    public RuleViolation (IRule rule, IRuleTarget target, string message)
    {
      Rule = rule;
      Target = target;
      Message = message;
    }
  }
}