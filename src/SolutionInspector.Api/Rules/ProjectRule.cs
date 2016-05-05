using System;
using System.Configuration;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector rule that targets <see cref="IProject" />s.
  /// </summary>
  public interface IProjectRule : IRule<IProject>
  {
  }

  /// <inheritdoc />
  public abstract class ProjectRule : Rule<IProject>, IProjectRule
  {
  }

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