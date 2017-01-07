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
    string RuleType { get; }
    XElement Element { get; }
  }

  /// <inheritdoc cref="IRuleConfiguration"/>>
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