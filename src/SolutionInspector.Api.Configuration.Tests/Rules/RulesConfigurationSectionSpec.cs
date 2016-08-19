using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Configuration.Tests.Rules
{
  [Subject (typeof(RulesConfigurationElement))]
  class RulesConfigurationSectionSpec
  {
    static RulesConfigurationElement SUT;

    class when_deserializing_config
    {
      Because of = () => SUT = ConfigurationElement.Load<RulesConfigurationElement>(XElement.Parse(@"<rules>
  <solutionRules>
    <rule type=""Namespace.Rule, Assembly"" property=""Property"">
      <innerConfig property=""InnerProperty"" />
    </rule>
  </solutionRules>
  <projectRules>
    <projectRuleGroup appliesTo=""*"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectRuleGroup>
    <projectRuleGroup appliesTo=""+Inc*lude;-Exc*lude"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectRuleGroup>
    <projectRuleGroup appliesTo=""Project"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectRuleGroup>
  </projectRules>
  <projectItemRules>
    <projectItemRuleGroup appliesTo=""App.config"" inProject=""Project"">
      <rule type=""Namespace.Rule, Assembly"" property=""Property"">
        <innerConfig property=""InnerProperty"" />
      </rule>
    </projectItemRuleGroup>
  </projectItemRules>
</rules>"));

      It reads_solution_rules = () =>
          AssertRule(SUT.SolutionRules.Single());

      It reads_project_rule_groups = () =>
      {
        var allGroup = SUT.ProjectRuleGroups[0];
        AssertRule(allGroup.Rules.Single());

        var filterGroup = SUT.ProjectRuleGroups[1];
        AssertRule(filterGroup.Rules.Single());

        var exactGroup = SUT.ProjectRuleGroups[2];
        AssertRule(exactGroup.Rules.Single());
      };

      It reads_file_rules_groups = () =>
      {
        var exactGroup = SUT.ProjectItemRuleGroups[0];
        AssertRule(exactGroup.Rules.Single());
      };
    }

    static void AssertRule (RuleConfigurationElement rule)
    {
      rule.RuleType.Should().Be("Namespace.Rule, Assembly");
    }
  }
}