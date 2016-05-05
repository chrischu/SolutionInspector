using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SolutionInspector.Api.Configuration.Infrastructure;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.Configuration.Rules
{
  /// <summary>
  ///   Configuration defining a set of <see cref="Rules" /> that is applied to all the projects that match the <see cref="AppliesTo" /> filter.
  /// </summary>
  public interface IProjectRuleGroupConfiguration
  {
    /// <summary>
    ///   Filter that controlls which projects are affected by the rules in the <see cref="IProjectRuleGroupConfiguration" />.
    /// </summary>
    INameFilter AppliesTo { get; }

    /// <summary>
    ///   The set of rules that is applied to all projects that match the <see cref="AppliesTo" /> filter.
    /// </summary>
    IReadOnlyCollection<IRuleConfiguration> Rules { get; }
  }

  internal class ProjectRuleGroupConfigurationElement
      : RuleConfigurationCollection, IKeyedConfigurationElement<string>, IProjectRuleGroupConfiguration
  {
    [ConfigurationProperty ("appliesTo", IsRequired = true, DefaultValue = "*")]
    // ReSharper disable once MemberCanBePrivate.Global
    public NameFilter AppliesTo => (NameFilter) this["appliesTo"];

    public IReadOnlyCollection<IRuleConfiguration> Rules => this.ToArray();

    INameFilter IProjectRuleGroupConfiguration.AppliesTo => AppliesTo;

    public string Key => AppliesTo.ToString();
    public string KeyName => "appliesTo";
  }
}