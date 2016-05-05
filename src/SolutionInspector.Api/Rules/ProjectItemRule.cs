using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector rule that targets <see cref="IProjectItem" />s.
  /// </summary>
  public interface IProjectItemRule : IRule<IProjectItem>
  {
  }

  /// <inheritdoc />
  [PublicAPI]
  public abstract class ProjectItemRule : Rule<IProjectItem>, IProjectItemRule
  {
  }

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