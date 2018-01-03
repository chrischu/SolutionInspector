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
  public class SolutionBuildConfigurationsRule : SolutionRule
  {
    private readonly ICollectionDifferenceFinder _collectionDifferenceFinder;

    /// <inheritdoc />
    public SolutionBuildConfigurationsRule ()
    {
      _collectionDifferenceFinder = new CollectionDifferenceFinder();
    }

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

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate ([NotNull] ISolution target)
    {
      var expectedConfigurations = from config in Configurations
          from platform in Platforms
          select new BuildConfiguration(config, platform);

      var differences = _collectionDifferenceFinder.FindDifferences(expectedConfigurations, target.BuildConfigurations);

      foreach (var add in differences.Adds)
        yield return new RuleViolation(this, target, $"Unexpected build configuration '{add}' found.");

      foreach (var remove in differences.Removes)
        yield return new RuleViolation(this, target, $"Build configuration '{remove}' could not be found.");
    }
  }
}