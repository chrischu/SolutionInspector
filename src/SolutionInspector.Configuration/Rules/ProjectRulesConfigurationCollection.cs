using System;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration;

namespace SolutionInspector.Configuration.Rules
{
  [UsedImplicitly]
  internal class ProjectRulesConfigurationCollection : KeyedConfigurationElementCollectionBase<ProjectRuleGroupConfigurationElement, string>
  {
    protected override string ElementName => "projectRuleGroup";
  }
}