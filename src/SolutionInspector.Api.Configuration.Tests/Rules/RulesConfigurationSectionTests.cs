using System.Xml.Linq;
using FluentAssertions;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using Xunit;

namespace SolutionInspector.Api.Configuration.Tests.Rules
{
  public class RulesConfigurationSectionTests
  {
    [Fact]
    public void Loading ()
    {
      var element = XElement.Parse(@"<rules>
  <solutionRules>
    <rule type=""Namespace.SolutionRule, Assembly"" />
  </solutionRules>
  <projectRules>
    <projectRuleGroup appliesTo=""Project"">
      <rule type=""Namespace.ProjectRule, Assembly"" />
    </projectRuleGroup>
  </projectRules>
  <projectItemRules>
    <projectItemRuleGroup appliesTo=""Item"" inProject=""Project"">
      <rule type=""Namespace.ProjectItemRule, Assembly"" />
    </projectItemRuleGroup>
  </projectItemRules>
</rules>");

      // ACT
      var result = ConfigurationElement.Load<RulesConfigurationElement>(element);

      // ASSERT
      result.SolutionRules.Count.Should().Be(1);
      result.SolutionRules[0].RuleType.Should().Be("Namespace.SolutionRule, Assembly");

      result.ProjectRuleGroups.Count.Should().Be(1);
      result.ProjectRuleGroups[0].AppliesTo.ToString().Should().Be("+Project");
      result.ProjectRuleGroups[0].Rules.Count.Should().Be(1);
      result.ProjectRuleGroups[0].Rules[0].RuleType.Should().Be("Namespace.ProjectRule, Assembly");

      result.ProjectItemRuleGroups.Count.Should().Be(1);
      result.ProjectItemRuleGroups[0].AppliesTo.ToString().Should().Be("+Item");
      result.ProjectItemRuleGroups[0].InProject.ToString().Should().Be("+Project");
      result.ProjectItemRuleGroups[0].Rules.Count.Should().Be(1);
      result.ProjectItemRuleGroups[0].Rules[0].RuleType.Should().Be("Namespace.ProjectItemRule, Assembly");
    }
  }
}