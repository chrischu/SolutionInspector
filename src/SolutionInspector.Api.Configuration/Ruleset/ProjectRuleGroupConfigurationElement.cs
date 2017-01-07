using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for a group of <see cref="ProjectRule" />s.
  /// </summary>
  public interface IProjectRuleGroupConfiguration
  {
    INameFilter AppliesTo { get; }
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  /// <inheritdoc cref="IProjectRuleGroupConfiguration" />
  /// >
  public class ProjectRuleGroupConfigurationElement : ConfigurationElement, IProjectRuleGroupConfiguration
  {
    [ConfigurationValue]
    public NameFilter AppliesTo
    {
      get { return GetConfigurationValue<NameFilter>(); }
      set { SetConfigurationValue(value); }
    }

    [ConfigurationCollection (IsDefaultCollection = true, ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> Rules => GetConfigurationCollection<RuleConfigurationElement>();

    [ExcludeFromCodeCoverage]
    INameFilter IProjectRuleGroupConfiguration.AppliesTo => AppliesTo;

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IRuleConfiguration> IProjectRuleGroupConfiguration.Rules => Rules;
  }
}