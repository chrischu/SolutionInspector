using System;
using System.Xml.Linq;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.Internals
{
  /// <summary>
  ///   Utility to instantiate <see cref="IRule" />s.
  /// </summary>
  public interface IRuleInstantiator
  {
    RuleConfigurationElement Instantiate (string ruleTypeName, XElement configurationElement);
  }

  internal class RuleInstantiator : IRuleInstantiator
  {
    public RuleConfigurationElement Instantiate (string ruleTypeName, XElement configurationElement)
    {
      var ruleType = ResolveRuleType(ruleTypeName);

      return (RuleConfigurationElement) ConfigurationElement.Load(ruleType, configurationElement);
    }

    private static Type ResolveRuleType (string ruleTypeName)
    {
      var ruleType = Type.GetType(ruleTypeName);

      if (ruleType == null)
        throw new RuleInstantiationException($"Could not resolve rule type '{ruleTypeName}'.");

      if (!typeof(IRule).IsAssignableFrom(ruleType))
        throw new RuleInstantiationException($"The type '{ruleType.Name}' is not a valid rule type.");

      return ruleType;
    }
  }
}