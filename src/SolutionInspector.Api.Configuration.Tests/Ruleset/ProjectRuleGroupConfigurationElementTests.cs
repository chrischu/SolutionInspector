
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Configuration.Tests.Ruleset
{
  public class ProjectRuleGroupConfigurationElementTests
  {
    [Test]
    public void AppliesToSet ()
    {
      var element = XElement.Parse(@"<projectRuleGroup appliesTo=""Original"" />");
      var projectRuleGroup = ConfigurationElement.Load<ProjectRuleGroupConfigurationElement>(element);

      // ACT
      projectRuleGroup.AppliesTo = new NameFilter(new[] { "Chan" }, new[] { "ged" });

      // ASSERT
      projectRuleGroup.AppliesTo.ToString().Should().Be("+Chan;-ged");
      element.Attribute("appliesTo").AssertNotNull().Value.Should().Be("+Chan;-ged");
    }
  }

  public class ProjectItemRuleGroupConfigurationElementTests
  {
    [Test]
    public void AppliesToSet()
    {
      var element = XElement.Parse(@"<projectItemRuleGroup appliesTo=""Item"" inProject=""Project"" />");
      var projectItemRuleGroup = ConfigurationElement.Load<ProjectItemRuleGroupConfigurationElement>(element);

      // ACT
      projectItemRuleGroup.AppliesTo = new NameFilter(new[] { "Chan" }, new[] { "ged" });

      // ASSERT
      projectItemRuleGroup.AppliesTo.ToString().Should().Be("+Chan;-ged");
      element.Attribute("appliesTo").AssertNotNull().Value.Should().Be("+Chan;-ged");
    }

    [Test]
    public void InProjectSet()
    {
      var element = XElement.Parse(@"<projectItemRuleGroup appliesTo=""Item"" inProject=""Project"" />");
      var projectItemRuleGroup = ConfigurationElement.Load<ProjectItemRuleGroupConfigurationElement>(element);

      // ACT
      projectItemRuleGroup.InProject = new NameFilter(new[] { "Chan" }, new[] { "ged" });

      // ASSERT
      projectItemRuleGroup.InProject.ToString().Should().Be("+Chan;-ged");
      element.Attribute("inProject").AssertNotNull().Value.Should().Be("+Chan;-ged");
    }
  }
}