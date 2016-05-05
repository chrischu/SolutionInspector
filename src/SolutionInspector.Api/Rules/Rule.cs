using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  /// A SolutionInspector rule.
  /// </summary>
  public interface IRule
  {
  }

  /// <summary>
  /// A SolutionInspector rule that applies to the given <typeparamref name="TTarget"/>.
  /// </summary>
  [PublicAPI]
  public interface IRule<in TTarget> : IRule
      where TTarget : IRuleTarget
  {
    /// <summary>
    /// Evaluates the rule against the given <paramref name="target"/> and returns all <see cref="IRuleViolation"/>s that were found.
    /// </summary>
    IEnumerable<IRuleViolation> Evaluate(TTarget target);
  }

  /// <inheritdoc />
  public abstract class Rule<TTarget> : IRule
      where TTarget : IRuleTarget
  {
    /// <summary>
    /// Evaluates the rule against the given <paramref name="target"/> and returns all <see cref="IRuleViolation"/>s that were found.
    /// </summary>
    public abstract IEnumerable<IRuleViolation> Evaluate(TTarget target);
  }
}