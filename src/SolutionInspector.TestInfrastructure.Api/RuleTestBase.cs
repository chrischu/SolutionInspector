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
      var document = new XDocument();
      var element = new XElement("rule");
      document.Add(element);
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