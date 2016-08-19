using System.Configuration;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <inheritdoc />
  public abstract class ConfigurableProjectItemRule<TConfiguration> : ConfigurableRule<IProjectItem, TConfiguration>, IProjectItemRule
      where TConfiguration : ConfigurationElement
  {
    /// <inheritdoc />
    protected ConfigurableProjectItemRule (TConfiguration configuration)
        : base(configuration)
    {
    }
  }
}