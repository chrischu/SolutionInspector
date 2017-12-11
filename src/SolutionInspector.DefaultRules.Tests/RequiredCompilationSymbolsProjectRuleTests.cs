using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration;

namespace SolutionInspector.DefaultRules.Tests
{
  public class RequiredCompilationSymbolsProjectRuleTests
  {
    private IAdvancedProject _advancedProject;
    private BuildConfiguration _filteredBuildConfiguration;
    private BuildConfiguration _includedBuildConfiguration;
    private IProject _project;

    private RequiredCompilationSymbolsProjectRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _includedBuildConfiguration = new BuildConfiguration("Included", "Platform");
      _filteredBuildConfiguration = new BuildConfiguration("Filtered", "Platform");

      _advancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => _project.Advanced).Returns(_advancedProject);
      A.CallTo(() => _project.BuildConfigurations).Returns(new[] { _includedBuildConfiguration, _filteredBuildConfiguration });

      var requiredCompilationSymbolsProjectRuleConfiguration = ConfigurationElement.Create<RequiredCompilationSymbolsProjectRuleConfiguration>(
        initialize: e =>
        {
          var item = e.RequiredCompilationSymbols.AddNew();
          item.BuildConfigurationFilter = new BuildConfigurationFilter(new BuildConfiguration("Included", "*"));
          item.RequiredCompilationSymbols.AssertNotNull().Add("TRACE", "DEBUG");
        });

      _sut = new RequiredCompilationSymbolsProjectRule(requiredCompilationSymbolsProjectRuleConfiguration);
    }

    [Test]
    public void Evaluate_AllRequiredSymbolsAreThere_ReturnsNoViolations ()
    {
      var property = A.Fake<IEvaluatedProjectPropertyValue>();
      A.CallTo(() => property.Value).Returns("TRACE;DEBUG");
      A.CallTo(() => _advancedProject.EvaluateProperties(_includedBuildConfiguration, null))
          .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property } });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_ARequiredSymbolIsMissing_ReturnsViolation ()
    {
      var property = A.Fake<IEvaluatedProjectPropertyValue>();
      A.CallTo(() => property.Value).Returns("TRACE");
      A.CallTo(() => _advancedProject.EvaluateProperties(_includedBuildConfiguration, null))
          .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property } });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(
            _sut,
            _project,
            $"In the build configuration '{_includedBuildConfiguration}' the required compilation symbol 'DEBUG' was not found.")
        });
    }

    [Test]
    public void Evaluate_ARequiredSymbolIsMissingInAFilteredConfiguration_ReturnsNoViolations ()
    {
      var property = A.Fake<IEvaluatedProjectPropertyValue>();
      A.CallTo(() => property.Value).Returns("TRACE;DEBUG");
      A.CallTo(() => _advancedProject.EvaluateProperties(_includedBuildConfiguration, null))
          .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property } });

      var property2 = A.Fake<IEvaluatedProjectPropertyValue>();
      A.CallTo(() => property2.Value).Returns("TRACE");
      A.CallTo(() => _advancedProject.EvaluateProperties(_filteredBuildConfiguration, null))
          .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property2 } });

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }
  }
}