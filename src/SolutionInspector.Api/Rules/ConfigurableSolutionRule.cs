using System;
using System.Configuration;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
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