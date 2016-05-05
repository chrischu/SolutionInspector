using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.Rules
{
  [UsedImplicitly]
  [ConfigurationCollection(typeof(ProjectRuleGroupConfigurationElement), AddItemName = "projectRuleGroup",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
  internal class ProjectRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectRuleGroupConfigurationElement, string>
  {
  }
}