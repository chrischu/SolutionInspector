using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Configuration;
using Wrapperator.Interfaces.Xml.Linq;

namespace SolutionInspector.Api.Configuration.Tests.Ruleset
{
  public class RulesetConfigurationDocumentTests
  {
    private IXDocumentStatic _xDocumentStatic;
    private IConfigurationManager _configurationManager;

    [SetUp]
    public void SetUp ()
    {
      _xDocumentStatic = A.Fake<IXDocumentStatic>();

      _configurationManager = new ConfigurationManager(_xDocumentStatic);
    }

    [Test]
    public void Load ()
    {
      var xDocument = XDocument.Parse(@"
<ruleset>
  <ruleAssemblyImports>
    <import path=""C:\A.dll"" />
    <import path=""C:\Assemblies"" />
  </ruleAssemblyImports>
  <rules>
    <solutionRules>
      <rule type=""Namespace.SolutionRule, SolutionAssembly"" />
    </solutionRules>
    <projectRules>
      <projectRuleGroup appliesTo=""Project"">
        <rule type=""Namespace.ProjectRule, ProjectAssembly"" />
      </projectRuleGroup>
    </projectRules>
    <projectItemRules>
      <projectItemRuleGroup appliesTo=""Item"" inProject=""Project"">
        <rule type=""Namespace.ProjectItemRule, ProjectItemAssembly"" />
      </projectItemRuleGroup>
    </projectItemRules>
  </rules>
</ruleset>");

      A.CallTo(() => _xDocumentStatic.Load(A<string>._))
          .Returns(A.Fake<IXDocument>(o => o.ConfigureFake(x => A.CallTo(() => x.Document).Returns(xDocument))));

      // ACT
      var result = _configurationManager.LoadDocument<RulesetConfigurationDocument>("path");

      // ASSERT
      result.RuleAssemblyImports.ShouldBeEquivalentTo(
        new[]
        {
          new { Path = @"C:\A.dll" },
          new { Path = @"C:\Assemblies" }
        },
        o => o.ExcludingMissingMembers());

      result.Rules.SolutionRules.ShouldBeEquivalentTo(
        new[]
        {
          new { RuleType = "Namespace.SolutionRule, SolutionAssembly" }
        },
        o => o.ExcludingMissingMembers());

      result.Rules.ProjectRuleGroups.ShouldBeEquivalentTo(
        new[]
        {
          new
          {
            AppliesTo = "Project",
            Rules = new[]
                    {
                      new { RuleType = "Namespace.ProjectRule, ProjectAssembly" }
                    }
          }
        }, o => o.ExcludingMissingMembers());

      result.Rules.ProjectItemRuleGroups.ShouldBeEquivalentTo(
        new[]
        {
          new
          {
            AppliesTo = "Item",
            InProject = "Project",
            Rules = new[]
                    {
                      new { RuleType = "Namespace.ProjectItemRule, ProjectItemAssembly" }
                    }
          }
        }, o => o.ExcludingMissingMembers());
    }
  }
}