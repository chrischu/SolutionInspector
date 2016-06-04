using System;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.Rules;
using SolutionInspector.TestInfrastructure.Configuration;

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

namespace SolutionInspector.Api.Tests.Configuration.Rules
{
  [Subject (typeof(RulesConfigurationSection))]
  class RulesConfigurationSectionSpec
  {
    static RulesConfigurationSection SUT;

    Establish ctx = () => { SUT = new RulesConfigurationSection(); };

    class when_deserializing_config
    {
      Because of = () => ConfigurationHelper.DeserializeSection(SUT, RulesConfigurationSection.ExampleConfiguration);

      It reads_solution_rules = () =>
          AssertRule(SUT.SolutionRules.Single());

      It reads_project_rule_groups = () =>
      {
        var allGroup = SUT.ProjectRules.GetElement("+*");
        AssertRule(allGroup.Rules.Single());

        var filterGroup = SUT.ProjectRules.GetElement("+Inc*lude;-Exc*lude");
        AssertRule(filterGroup.Rules.Single());

        var exactGroup = SUT.ProjectRules.GetElement("+Project");
        AssertRule(exactGroup.Rules.Single());
      };

      It reads_file_rules_groups = () =>
      {
        var exactGroup = SUT.ProjectItemRules.GetElement("+Project +App.config");
        AssertRule(exactGroup.Rules.Single());
      };
    }

    static void AssertRule (IRuleConfiguration rule)
    {
      rule.RuleType.Should().Be("Namespace.Rule, Assembly");
      rule.Configuration.OuterXml.Should().Be(@"<rule property=""Property""><innerConfig property=""InnerProperty"" /></rule>");
    }
  }
}