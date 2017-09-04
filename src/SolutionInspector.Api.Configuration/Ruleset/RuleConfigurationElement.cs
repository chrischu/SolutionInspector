using System;
using System.Xml.Linq;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  /// <summary>
  ///   Configuration for a <see cref="Rule{TTarget}" />.
  /// </summary>
  public interface IRuleConfiguration
  {
    /// <summary>
    ///   The assembly-qualified type name of the rule.
    /// </summary>
    string RuleType { get; }

    /// <summary>
    ///   The <see cref="XElement" /> representation of the rule configuration.
    /// </summary>
    XElement Element { get; }
  }

  /// <inheritdoc cref="IRuleConfiguration" />
  public class RuleConfigurationElement : ConfigurationElement, IRuleConfiguration
  {
    [ConfigurationValue(AttributeName = "type")]
    public string RuleType
    {
      get { return GetConfigurationValue<string>(); }
      set { SetConfigurationValue(value); }
    }
  }
}