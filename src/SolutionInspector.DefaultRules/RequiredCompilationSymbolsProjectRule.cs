using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;
using SolutionInspector.Api.Extensions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all the compilation symbols (configured in <see cref="RequiredCompilationSymbolsProjectRuleConfiguration" />) are configured in the
  ///   project.
  /// </summary>
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
      foreach (var config in Configuration)
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
  public class RequiredCompilationSymbolsProjectRuleConfiguration
      : KeyedConfigurationElementCollectionBase<RequiredCompilationSymbolsConfigurationElement, BuildConfigurationFilter>
  {
    protected override string ElementName => "requiredCompilationSymbols";
  }

  /// <summary>
  ///   Configuration for which compilation symbols (<see cref="RequiredCompilationSymbols" />) are required in the build configurations matching the
  ///   <see cref="BuildConfigurationFilter" />.
  /// </summary>
  public class RequiredCompilationSymbolsConfigurationElement : KeyedConfigurationElement<BuildConfigurationFilter>
  {
    /// <inheritdoc />
    public override string KeyName => "buildConfigurationFilter";

    /// <summary>
    ///   Filter that controlls which build configuration this <see cref="RequiredCompilationSymbolsConfigurationElement" /> applies to.
    /// </summary>
    [TypeConverter (typeof(BuildConfigurationFilterConverter))]
    [ConfigurationProperty ("buildConfigurationFilter", DefaultValue = "*|*", IsRequired = true)]
    public BuildConfigurationFilter BuildConfigurationFilter
    {
      get { return (BuildConfigurationFilter) this["buildConfigurationFilter"]; }
      [UsedImplicitly] set { this["buildConfigurationFilter"] = value; }
    }

    /// <summary>
    ///   All the compilation symbols that are required and are therefore checked.
    /// </summary>
    [TypeConverter (typeof(CommaDelimitedStringCollectionConverter))]
    [ConfigurationProperty ("requiredCompilationSymbols", DefaultValue = "", IsRequired = true)]
    public CommaDelimitedStringCollection RequiredCompilationSymbols
    {
      get { return (CommaDelimitedStringCollection) this["requiredCompilationSymbols"]; }
      [UsedImplicitly] set { this["requiredCompilationSymbols"] = value; }
    }
  }
}