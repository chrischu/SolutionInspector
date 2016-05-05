using System;
using System.Configuration;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Infrastructure;

namespace SolutionInspector.Api.Configuration.Rules
{
  [UsedImplicitly]
  internal class ProjectItemRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectItemRuleGroupConfigurationElement, string>
  {
    protected override string ElementName => "projectItemRuleGroup";
  }
}