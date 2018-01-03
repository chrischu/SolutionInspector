using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.Api.Tests.Configuration.Ruleset
{
  public class ProjectRuleGroupConfigurationElementTests
  {
    [Test]
    public void NameSet()
    {
      var element = XElement.Parse(@"<projectRuleGroup name=""Name"" appliesTo=""*"" />");
      var projectRuleGroup = ConfigurationElement.Load<ProjectRuleGroupConfigurationElement>(element);

      // ACT
      projectRuleGroup.Name = "Changed";

      // ASSERT
      projectRuleGroup.Name.Should().Be("Changed");
      element.Attribute("name").AssertNotNull().Value.Should().Be("Changed");
    }

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
    public void NameSet()
    {
      var element = XElement.Parse(@"<projectItemRuleGroup name=""Name"" appliesTo=""*"" inProject=""*"" />");
      var projectItemRuleGroup = ConfigurationElement.Load<ProjectItemRuleGroupConfigurationElement>(element);

      // ACT
      projectItemRuleGroup.Name = "Changed";

      // ASSERT
      projectItemRuleGroup.Name.Should().Be("Changed");
      element.Attribute("name").AssertNotNull().Value.Should().Be("Changed");
    }

    [Test]
    public void AppliesToSet ()
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
    public void InProjectSet ()
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