using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Infrastructure;

namespace SolutionInspector.Configuration.Rules
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(ProjectItemRuleGroupConfigurationElement), AddItemName = "projectItemRuleGroup",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectItemRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectItemRuleGroupConfigurationElement, string>
  {
  }
}