using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SolutionInspector.Api.Configuration.Infrastructure;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.Configuration.Rules
{
  /// <summary>
  /// Configuration defining a set of <see cref="Rules"/> that is applied to all the project items that match the <see cref="AppliesTo"/> and <see cref="InProject"/> filters.
  /// </summary>
  public interface IProjectItemRuleGroupConfiguration
  {
    /// <summary>
    /// Filter that controlls which project items are affected by the rules in the <see cref="IProjectItemRuleGroupConfiguration"/>.
    /// </summary>
    INameFilter AppliesTo { get; }

    /// <summary>
    /// Filter that controlls which project's items are affected by the rules in the <see cref="IProjectItemRuleGroupConfiguration"/>.
    /// </summary>
    INameFilter InProject { get; }

    /// <summary>
    /// The set of rules that is applied to all project items that match the <see cref="AppliesTo"/> and <see cref="InProject"/> filters.
    /// </summary>
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  [ConfigurationCollection(typeof (RuleConfigurationElement), AddItemName = "rule", CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectItemRuleGroupConfigurationElement : RuleConfigurationCollection, IKeyedConfigurationElement<string>, IProjectItemRuleGroupConfiguration
  {
    [ConfigurationProperty("appliesTo", IsRequired = true, DefaultValue = "*")]
    // ReSharper disable once MemberCanBePrivate.Global
    public NameFilter AppliesTo => (NameFilter)this["appliesTo"];

    [ConfigurationProperty("inProject", IsRequired = true, DefaultValue = "*")]
    // ReSharper disable once MemberCanBePrivate.Global
    public NameFilter InProject => (NameFilter)this["inProject"];

    public IReadOnlyCollection<IRuleConfiguration> Rules => this.ToArray();

    INameFilter IProjectItemRuleGroupConfiguration.AppliesTo => AppliesTo;
    INameFilter IProjectItemRuleGroupConfiguration.InProject => InProject;

    public string Key => $"{InProject} {AppliesTo}";
    public string KeyName => "appliesTo";
  }
}