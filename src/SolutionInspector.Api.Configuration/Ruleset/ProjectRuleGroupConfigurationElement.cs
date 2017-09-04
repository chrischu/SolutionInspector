using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Attributes;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for a group of <see cref="ProjectRule" />s.
  /// </summary>
  [PublicApi]
  public interface IProjectRuleGroupConfiguration
  {
    /// <summary>
    ///   The name of the <see cref="IProjectRuleGroupConfiguration" />.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The <see cref="INameFilter" /> that controls to which <see cref="IProject" />s the <see cref="IProjectRuleGroupConfiguration" /> applies.
    /// </summary>
    INameFilter AppliesTo { get; }

    /// <summary>
    ///   The collection of rules in the <see cref="IProjectRuleGroupConfiguration" />.
    /// </summary>
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  /// <inheritdoc cref="IProjectRuleGroupConfiguration" />
  public class ProjectRuleGroupConfigurationElement : ConfigurationElement, IProjectRuleGroupConfiguration
  {
    [ConfigurationValue(IsOptional = true)]
    public string Name
    {
      get { return GetConfigurationValue<string>(); }
      set { SetConfigurationValue(value); }
    }

    /// <see cref="IProjectRuleGroupConfiguration.AppliesTo"/>
    [ConfigurationValue]
    public NameFilter AppliesTo
    {
      get { return GetConfigurationValue<NameFilter>(); }
      set { SetConfigurationValue(value); }
    }

    /// <see cref="IProjectRuleGroupConfiguration.Rules"/>
    [ConfigurationCollection(IsDefaultCollection = true, ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> Rules => GetConfigurationCollection<RuleConfigurationElement>();

    [ExcludeFromCodeCoverage]
    INameFilter IProjectRuleGroupConfiguration.AppliesTo => AppliesTo;

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IRuleConfiguration> IProjectRuleGroupConfiguration.Rules => Rules;
  }
}