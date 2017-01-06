using System;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.DefaultRules.Tests
{
  public class ProjectXPathRuleTests
  {
    private IProject _project;

    private ProjectXPathRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();
      A.CallTo(() => _project.ProjectXml).Returns(XDocument.Parse("<xml><element attr=\"7\" /></xml>"));

      _sut = new ProjectXPathRule(
               new ProjectXPathRuleConfiguration
               {
                 XPath = "boolean(//element[@attr=7])"
               });
    }

    [Test]
    public void Evaluate_XPathExpressionThatEvaluatesToTrue_ReturnsNoViolations ()
    {
      _sut = new ProjectXPathRule(
               new ProjectXPathRuleConfiguration
               {
                 XPath = "boolean(//element[@attr=7])"
               });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_XPathExpressionThatEvaluatesToFalse_ReturnsViolation ()
    {
      _sut = new ProjectXPathRule(
               new ProjectXPathRuleConfiguration
               {
                 XPath = "boolean(//nonExistingElement)"
               });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, $"The XPath expression '{_sut.Configuration.XPath}' did not evaluate to 'true', but to 'false'.")
        });
    }

    [Test]
    public void Evaluate_XPathExpressionThatDoesNotEvaluateToBoolean_Throws ()
    {
      _sut = new ProjectXPathRule(
               new ProjectXPathRuleConfiguration
               {
                 XPath = "//element"
               });

      // ACT
      Action act = () => Dev.Null = _sut.Evaluate(_project).ToArray();

      // ASSERT
      act.ShouldThrow<ProjectXPathRule.InvalidXPathExpressionException>()
          .WithMessage($"The configured XPath expression '{_sut.Configuration.XPath}' does not evaluate to a boolean value.");
    }

    [Test]
    public void Evaluate_DocumentWithNamespaces_IgnoresNamespaces ()
    {
      _sut = new ProjectXPathRule(
               new ProjectXPathRuleConfiguration
               {
                 XPath = "boolean(//element)"
               });

      A.CallTo(() => _project.ProjectXml)
          .Returns(XDocument.Parse("<xml xmlns=\"http://some.namespace\" xmlns:x=\"http://some.other.namespace\"><element /></xml>"));

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_DocumentWithNamespacesAndNamespaceIgnoringIsDisabled_ReturnsViolation ()
    {
      _sut = new ProjectXPathRule(
               new ProjectXPathRuleConfiguration
               {
                 XPath = "boolean(//element)",
                 IgnoreNamespaces = false
               });

      A.CallTo(() => _project.ProjectXml).Returns(XDocument.Parse("<xml xmlns=\"http://some.namespace\"><element /></xml>"));

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, $"The XPath expression '{_sut.Configuration.XPath}' did not evaluate to 'true', but to 'false'.")
        });
    }
  }
}