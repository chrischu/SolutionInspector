using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Infrastructure;

namespace SolutionInspector.Configuration.Rules
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(RuleConfigurationElement), AddItemName = "rule")]
  internal class RuleConfigurationCollection : KeyedConfigurationElementCollectionBase<RuleConfigurationElement, string>
  {

  }
}