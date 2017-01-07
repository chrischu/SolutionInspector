using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc />
  public abstract class ConfigurableProjectRule<TConfiguration> : ConfigurableRule<IProject, TConfiguration>, IProjectRule
    where TConfiguration : ConfigurationElement
  {
    /// <inheritdoc />
    protected ConfigurableProjectRule (TConfiguration configuration)
      : base(configuration)
    {
    }
  }
}