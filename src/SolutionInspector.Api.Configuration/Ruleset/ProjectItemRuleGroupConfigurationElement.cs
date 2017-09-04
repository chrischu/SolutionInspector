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
  ///   Configuration for a group of <see cref="ProjectItemRule" />s.
  /// </summary>
  [PublicApi]
  public interface IProjectItemRuleGroupConfiguration
  {
    /// <summary>
    ///   The name of the <see cref="IProjectItemRuleGroupConfiguration" />.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The <see cref="INameFilter" /> that controls to which <see cref="IProjectItem" />s the <see cref="IProjectItemRuleGroupConfiguration" /> applies.
    /// </summary>
    INameFilter AppliesTo { get; }

    /// <summary>
    ///   The <see cref="INameFilter" /> that controls to which <see cref="IProject" />s the <see cref="IProjectItemRuleGroupConfiguration" /> applies.
    /// </summary>
    INameFilter InProject { get; }

    /// <summary>
    ///   The collection of rules in the <see cref="IProjectItemRuleGroupConfiguration" />.
    /// </summary>
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  /// <inheritdoc cref="IProjectItemRuleGroupConfiguration" />
  [ForFutureUse]
  public class ProjectItemRuleGroupConfigurationElement : ConfigurationElement, IProjectItemRuleGroupConfiguration
  {
    [ConfigurationValue(IsOptional = true)]
    public string Name
    {
      get { return GetConfigurationValue<string>(); }
      set { SetConfigurationValue(value); }
    }

    /// <see cref="IProjectItemRuleGroupConfiguration.AppliesTo"/>
    [ConfigurationValue]
    public NameFilter AppliesTo
    {
      get { return GetConfigurationValue<NameFilter>(); }
      set { SetConfigurationValue(value); }
    }

    /// <see cref="IProjectItemRuleGroupConfiguration.InProject"/>
    [ConfigurationValue]
    public NameFilter InProject
    {
      get { return GetConfigurationValue<NameFilter>(); }
      set { SetConfigurationValue(value); }
    }

    /// <see cref="IProjectItemRuleGroupConfiguration.Rules"/>
    [ConfigurationCollection(IsDefaultCollection = true, ElementName = "rule", IsOptional = true)]
    public ConfigurationElementCollection<RuleConfigurationElement> Rules => GetConfigurationCollection<RuleConfigurationElement>();

    [ExcludeFromCodeCoverage]
    INameFilter IProjectItemRuleGroupConfiguration.AppliesTo => AppliesTo;

    [ExcludeFromCodeCoverage]
    INameFilter IProjectItemRuleGroupConfiguration.InProject => InProject;

    [ExcludeFromCodeCoverage]
    IReadOnlyCollection<IRuleConfiguration> IProjectItemRuleGroupConfiguration.Rules => Rules;
  }
}