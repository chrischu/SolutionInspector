using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Utilities;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  ///   Verifies that all expected combinations of build configuration/platform are present in the solution.
  /// </summary>
  [Description ("Verifies that all expected combinations of build configuration/platform are present in the solution.")]
  public class SolutionBuildConfigurationsRule : ConfigurableSolutionRule<SolutionBuildConfigurationsRuleConfiguration>
  {
    private readonly ICollectionDifferenceFinder _collectionDifferenceFinder;
    private readonly Lazy<BuildConfiguration[]> _expectedConfigurations;

    /// <inheritdoc />
    public SolutionBuildConfigurationsRule (SolutionBuildConfigurationsRuleConfiguration configuration)
      : base(configuration)
    {
      _collectionDifferenceFinder = new CollectionDifferenceFinder();
      _expectedConfigurations = new Lazy<BuildConfiguration[]>(
        () => (from config in Configuration.Configurations
          from platform in Configuration.Platforms
          select new BuildConfiguration(config, platform)).ToArray());
    }

    /// <summary>
    ///   All the expected configurations.
    /// </summary>
    public IReadOnlyCollection<BuildConfiguration> ExpectedConfigurations => _expectedConfigurations.Value;

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] ISolution target)
    {
      var differences = _collectionDifferenceFinder.FindDifferences(ExpectedConfigurations, target.BuildConfigurations);

      foreach (var add in differences.Adds)
        yield return new RuleViolation(this, target, $"Unexpected build configuration '{add}' found.");

      foreach (var remove in differences.Removes)
        yield return new RuleViolation(this, target, $"Build configuration '{remove}' could not be found.");
    }
  }

  /// <summary>
  ///   Configuration for the <see cref="SolutionBuildConfigurationsRule" />.
  /// </summary>
  public class SolutionBuildConfigurationsRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    ///   "A list of expected configurations (e.g. 'Build', 'Release')."
    /// </summary>
    [CanBeNull]
    [ConfigurationValue (AttributeName = "expectedConfigurations", DefaultValue = "", IsOptional = false)]
    [Description ("A list of expected configurations (e.g. 'Build', 'Release').")]
    public CommaSeparatedStringCollection Configurations => GetConfigurationValue<CommaSeparatedStringCollection>();

    /// <summary>
    ///   A list of expected platforms (e.g. 'Any CPU', 'x64').
    /// </summary>
    [CanBeNull]
    [ConfigurationValue (AttributeName = "expectedPlatforms", DefaultValue = "", IsOptional = false)]
    [Description ("A list of expected platforms (e.g. 'Any CPU', 'x64).")]
    public CommaSeparatedStringCollection Platforms => GetConfigurationValue<CommaSeparatedStringCollection>();
  }
}