using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.Rules
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(ProjectItemRuleGroupConfigurationElement), AddItemName = "projectItemRuleGroup",
      CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectItemRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectItemRuleGroupConfigurationElement, string>
  {
  }
}