using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using SolutionInspector.ObjectModel;
using SolutionInspector.Rules;
using SolutionInspector.Utilities;

namespace SolutionInspector.DefaultRules
{
  /// <summary>
  /// Verifies that all expected combinations of build configuration/platform are present in the solution.
  /// </summary>
  public class SolutionBuildConfigurationsRule : ConfigurableSolutionRule<SolutionBuildConfigurationsRuleConfiguration>
  {
    private readonly ICollectionDifferenceFinder _collectionDifferenceFinder;
    private readonly Lazy<BuildConfiguration[]> _expectedConfigurations;

    internal IReadOnlyCollection<BuildConfiguration> ExpectedConfigurations => _expectedConfigurations.Value;

    /// <inheritdoc />
    public SolutionBuildConfigurationsRule(SolutionBuildConfigurationsRuleConfiguration configuration)
        : base(configuration)
    {
      _collectionDifferenceFinder = new CollectionDifferenceFinder();
      _expectedConfigurations = new Lazy<BuildConfiguration[]>(
          () => (from config in Configuration.Configurations.OfType<string>()
            from platform in Configuration.Platforms.OfType<string>()
            select new BuildConfiguration(config, platform)).ToArray());
    }

    /// <inheritdoc />
    public override IEnumerable<IRuleViolation> Evaluate(ISolution target)
    {
      var differences = _collectionDifferenceFinder.FindDifferences(ExpectedConfigurations, target.BuildConfigurations);

      foreach (var add in differences.Adds)
        yield return new RuleViolation(this, target, $"Unexpected build configuration '{add}' found.");

      foreach (var remove in differences.Removes)
        yield return new RuleViolation(this, target, $"Build configuration '{remove}' could not be found.");
    }
  }

  /// <summary>
  /// Configuration for the <see cref="SolutionBuildConfigurationsRule"/>.
  /// </summary>
  public class SolutionBuildConfigurationsRuleConfiguration : ConfigurationElement
  {
    /// <summary>
    /// A list of expected configurations (e.g. Build, Release).
    /// </summary>
    [TypeConverter(typeof (CommaDelimitedStringCollectionConverter))]
    [ConfigurationProperty("expectedConfigurations", DefaultValue = "", IsRequired = true)]
    public CommaDelimitedStringCollection Configurations
    {
      get { return (CommaDelimitedStringCollection) this["expectedConfigurations"]; }
      set { this["expectedConfigurations"] = value; }
    }

    /// <summary>
    /// A list of expected platforms (e.g. AnyCPU, x64).
    /// </summary>
    [TypeConverter(typeof (CommaDelimitedStringCollectionConverter))]
    [ConfigurationProperty("expectedPlatforms", DefaultValue = "", IsRequired = true)]
    public CommaDelimitedStringCollection Platforms
    {
      get { return (CommaDelimitedStringCollection) this["expectedPlatforms"]; }
      set { this["expectedPlatforms"] = value; }
    }
  }
}