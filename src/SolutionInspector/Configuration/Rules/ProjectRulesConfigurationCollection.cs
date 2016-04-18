using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Configuration.Infrastructure;

namespace SolutionInspector.Configuration.Rules
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(ProjectRuleGroupConfigurationElement), AddItemName = "projectRuleGroup",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectRuleGroupConfigurationElement, string>
  {
  }
}