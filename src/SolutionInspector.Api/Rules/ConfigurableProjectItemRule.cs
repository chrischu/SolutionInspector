using System;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A configurable <see cref="IProjectItemRule" />.
  /// </summary>
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