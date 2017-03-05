using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all the compilation symbols (configured in <see cref="RequiredCompilationSymbolsProjectRuleConfiguration" />) are configured in the
  ///   project.
  /// </summary>
  [Description ("Verifies that all the compilation symbols (see 'requiredCompilationSymbols') are configured in the project.")]
  public class RequiredCompilationSymbolsProjectRule : ConfigurableProjectRule<RequiredCompilationSymbolsProjectRuleConfiguration>
  {
    /// <inheritdoc />
    public RequiredCompilationSymbolsProjectRule ([NotNull] RequiredCompilationSymbolsProjectRuleConfiguration configuration)
      : base(configuration)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate (IProject target)
    {
      foreach (var config in Configuration.RequiredCompilationSymbols)
      {
        var matchingBuildConfigs = target.BuildConfigurations.Where(c => config.BuildConfigurationFilter.IsMatch(c));

        foreach (var matchingBuildConfig in matchingBuildConfigs)
        {
          var properties = target.Advanced.EvaluateProperties(matchingBuildConfig);

          var defineConstants = properties.GetValueOrDefault("DefineConstants")?.Value;
          var actualSymbols = new HashSet<string>(defineConstants?.Split(';') ?? Enumerable.Empty<string>());

          foreach (var requiredSymbol in config.RequiredCompilationSymbols)
            if (!actualSymbols.Contains(requiredSymbol))
              yield return
                  new RuleViolation(
                    this,
                    target,
                    $"In the build configuration '{matchingBuildConfig}' the required compilation symbol '{requiredSymbol}' was not found.");
        }
      }
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="RequiredCompilationSymbolsProjectRule" />.
  /// </summary>
  [UsedImplicitly /* by configuration */]
  public class RequiredCompilationSymbolsProjectRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   All the required compilation symbols.
    /// </summary>
    [ConfigurationCollection]
    public ConfigurationElementCollection<RequiredCompilationSymbolsConfigurationElement> RequiredCompilationSymbols
      => GetConfigurationCollection<RequiredCompilationSymbolsConfigurationElement>();
  }

  /// <summary>
  ///   Configuration for which compilation symbols (<see cref="RequiredCompilationSymbols" />) are required in the build configurations matching the
  ///   <see cref="BuildConfigurationFilter" />.
  /// </summary>
  public class RequiredCompilationSymbolsConfigurationElement : ConfigurationElement
  {
    /// <summary>
    ///   Filter that controlls which build configuration this <see cref="RequiredCompilationSymbolsConfigurationElement" /> applies to.
    /// </summary>
    [Description ("Filter that controlls which build configuration this rule applies to.")]
    [ConfigurationValue]
    public BuildConfigurationFilter BuildConfigurationFilter
    {
      get { return GetConfigurationValue<BuildConfigurationFilter>(); }
      [UsedImplicitly] set { SetConfigurationValue(value); }
    }

    /// <summary>
    ///   All the compilation symbols that are required and are therefore checked.
    /// </summary>
    [Description ("All the compilation symbols that are required and are therefore checked.")]
    [ConfigurationValue]
    public CommaSeparatedStringCollection RequiredCompilationSymbols => GetConfigurationValue<CommaSeparatedStringCollection>();
  }
}