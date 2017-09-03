using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A configurable <see cref="ISolutionRule" />.
  /// </summary>
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