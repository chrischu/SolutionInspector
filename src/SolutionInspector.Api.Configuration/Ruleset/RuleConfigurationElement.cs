using System.Xml.Linq;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Ruleset
{
  public interface IRuleConfiguration
  {
    string RuleType { get; }
    XElement Element { get; }
  }

  public class RuleConfigurationElement : ConfigurationElement, IRuleConfiguration
  {
    [ConfigurationValue (AttributeName = "type")]
    public string RuleType
    {
      get { return GetConfigurationValue<string>(); }
      set { SetConfigurationValue(value); }
    }
  }
}