using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector rule that targets <see cref="ISolution" />s.
  /// </summary>
  public interface ISolutionRule : IRule<ISolution>
  {
  }

  /// <inheritdoc />
  [PublicAPI]
  public abstract class SolutionRule : Rule<ISolution>, ISolutionRule
  {
  }

  /// <inheritdoc />
  public abstract class ConfigurableSolutionRule<TConfiguration> : ConfigurableRule<ISolution, TConfiguration>, ISolutionRule
      where TConfiguration : ConfigurationElement
  {
    /// <inheritdoc />
    protected ConfigurableSolutionRule (TConfiguration configuration)
        : base(configuration)
    {
    }
  }
}