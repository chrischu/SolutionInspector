using System;
using System.Xml.Linq;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;

namespace SolutionInspector.TestInfrastructure.Api
{
  /// <summary>
  ///   Base class for <see cref="IRule" /> tests.
  /// </summary>
  public abstract class RuleTestBase
  {
    protected T CreateRule<T> (Action<T> configure = null)
        where T : RuleConfigurationElement, new()
    {
      var element = new XElement("rule");
      element.SetAttributeValue("ruleType", "type");
      return ConfigurationElement.Load<T>(
          element,
          t =>
          {
            t.RuleType = "DONT CARE";
            configure?.Invoke(t);
          });
    }
  }
}