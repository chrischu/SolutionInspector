using System;
using System.Configuration;

namespace SolutionInspector.Api.Rules
{
  /// <summary>
  ///   A SolutionInspector that is configurable via C# configuration classes.
  /// </summary>
  public interface IConfigurableRule : IRule
  {
  }

  /// <summary>
  ///   A SolutionInspector rule that applies to the given <typeparamref name="TTarget" /> and can be configured using the
  ///   <typeparamref name="TConfiguration" />.
  /// </summary>
  public interface IConfigurableRule<in TTarget, out TConfiguration> : IRule<TTarget>, IConfigurableRule
      where TTarget : IRuleTarget
      where TConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   Configuration.
    /// </summary>
    TConfiguration Configuration { get; }
  }

  /// <inheritdoc />
  public abstract class ConfigurableRule<TTarget, TConfiguration> : Rule<TTarget>, IConfigurableRule<TTarget, TConfiguration>
      where TTarget : IRuleTarget
      where TConfiguration : ConfigurationElement
  {
    /// <inheritdoc />
    public TConfiguration Configuration { get; }

    /// <summary>
    ///   Creates an instance of the <see cref="ConfigurableRule{TTarget,TConfiguration}" /> with the given <paramref name="configuration" />.
    /// </summary>
    /// <param name="configuration"></param>
    protected ConfigurableRule (TConfiguration configuration)
    {
      Configuration = configuration;
    }
  }
}