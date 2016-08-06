using System;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;

namespace SolutionInspector.Configuration.Rules
{
  [UsedImplicitly]
  internal class ProjectItemRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectItemRuleGroupConfigurationElement, string>
  {
    protected override string ElementName => "projectItemRuleGroup";
  }
}