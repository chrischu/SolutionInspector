using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Build.Exceptions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Exceptions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commands;
using SolutionInspector.Internals;
using SolutionInspector.Reporting;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.Console.Commands;
using SolutionInspector.Utilities;

namespace SolutionInspector.Tests.Commands
{
  public class InspectCommandTests : CommandTestsBase
  {
    private IConfigurationLoader _configurationLoader;
    private IProject _project;
    private IProjectItem _projectItem;
    private IProjectItemRule _projectItemRule;
    private IProjectRule _projectRule;
    private IRuleAssemblyLoader _ruleAssemblyLoader;
    private IRuleCollectionBuilder _ruleCollectionBuilder;
    private IRuleCollection _rules;
    private IRulesConfiguration _rulesConfiguration;
    private IRulesetConfiguration _ruleset;
    private ISolution _solution;

    private ISolutionInspectorConfiguration _solutionInspectorConfiguration;

    private ISolutionLoader _solutionLoader;

    private ISolutionRule _solutionRule;

    private InspectCommand _sut;
    private IViolationReporter _violationReporter;
    private IViolationReporterFactory _violationReporterFactory;

    [SetUp]
    public new void SetUp ()
    {
      _solution = A.Fake<ISolution>();

      _project = A.Fake<IProject>();
      A.CallTo(() => _solution.Projects).Returns(new[] { _project });

      _projectItem = A.Fake<IProjectItem>();
      A.CallTo(() => _project.ProjectItems).Returns(new[] { _projectItem });

      _solutionLoader = A.Fake<ISolutionLoader>();
      A.CallTo(() => _solutionLoader.Load(A<string>._, A<IMsBuildParsingConfiguration>._)).Returns(_solution);

      _ruleCollectionBuilder = A.Fake<IRuleCollectionBuilder>();
      _solutionRule = A.Fake<ISolutionRule>();
      _projectRule = A.Fake<IProjectRule>();
      _projectItemRule = A.Fake<IProjectItemRule>();
      _rules = new RuleCollection(new[] { _solutionRule }, new[] { _projectRule }, new[] { _projectItemRule });
      A.CallTo(() => _ruleCollectionBuilder.Build(A<IRulesConfiguration>._)).Returns(_rules);

      _solutionInspectorConfiguration = A.Fake<ISolutionInspectorConfiguration>();
      A.CallTo(() => _solutionInspectorConfiguration.MsBuildParsing).Returns(A.Fake<IMsBuildParsingConfiguration>());

      _ruleset = A.Fake<IRulesetConfiguration>();

      _rulesConfiguration = A.Fake<IRulesConfiguration>();
      A.CallTo(() => _ruleset.Rules).Returns(_rulesConfiguration);

      _configurationLoader = A.Fake<IConfigurationLoader>();
      A.CallTo(() => _configurationLoader.LoadRulesConfig(A<string>._)).Returns(_ruleset);

      _ruleAssemblyLoader = A.Fake<IRuleAssemblyLoader>();
      _violationReporterFactory = A.Fake<IViolationReporterFactory>();

      _violationReporter = A.Fake<IViolationReporter>();
      A.CallTo(() => _violationReporterFactory.CreateConsoleReporter(A<ViolationReportFormat>._)).Returns(_violationReporter);
      A.CallTo(() => _violationReporterFactory.CreateFileReporter(A<ViolationReportFormat>._, A<string>._)).Returns(_violationReporter);

      _sut = new InspectCommand(
        _configurationLoader,
        _ruleAssemblyLoader,
        _solutionLoader,
        _ruleCollectionBuilder,
        _violationReporterFactory,
        _solutionInspectorConfiguration);
    }

    [Test]
    public void Run_WithoutExplicitSolutionConfigurationFile_LoadsDefaultConfiguration ()
    {
      // ACT
      var result = RunCommand(_sut, "solution");

      // ASSERT
      result.Should().Be(0);

      A.CallTo(() => _configurationLoader.LoadRulesConfig($"solution.{SolutionInspector.RulesetFileExtension}")).MustHaveHappened();

      AssertCorrectCommandExecution();
      AssertDoesNotCreateViolationReporter();
    }

    [Test]
    public void Run_WithExplicitSolutionConfigurationFile_LoadsSpecifiedConfiguration ()
    {
      // ACT
      var result = RunCommand(_sut, "--configurationFile=file", "solution");

      // ASSERT
      result.Should().Be(0);

      A.CallTo(() => _configurationLoader.LoadRulesConfig("file")).MustHaveHappened();

      AssertCorrectCommandExecution();
      AssertDoesNotCreateViolationReporter();
    }

    [Test]
    public void Run_AndConfigurationFileCannotBeFound_ShowsErrorAndAbortsCommand ()
    {
      A.CallTo(() => _configurationLoader.LoadRulesConfig(A<string>._)).Throws(new FileNotFoundException("NOTFOUND"));

      // ACT
      var result = RunCommand(_sut, "solution");

      // ASSERT
      result.Should().Be(-1);

      CapturedOutput.ToString().Should().Contain("NOTFOUND");
    }

    [Test]
    public void Run_AndConfigurationFileLoadingFails_ShowsErrorAndAbortsCommand ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _configurationLoader.LoadRulesConfig(A<string>._)).Throws(thrownException);

      // ACT
      var result = RunCommand(_sut, "solution");

      // ASSERT
      result.Should().Be(-1);

      CapturedOutput.ToString().Should().Contain($"Unexpected error when loading configuration file: {thrownException.Message}.");
    }

    [Test]
    public void Run_WithViolationsWithoutSpecifyingReport_CreatesAndCallsConsoleViolationReporter ()
    {
      var expectedViolations = SetupAndReturnSomeViolations();

      // ACT
      var result = RunCommand(_sut, "solution");

      // ASSERT
      result.Should().Be(1);

      AssertConfigurationLoadWithDefaultParameters();
      AssertCorrectCommandExecution();
      AssertCreatesAndCallsConsoleViolationReporterWithExpectedFormat(ViolationReportFormat.Xml, expectedViolations);
    }

    [Test]
    [TestCase (ViolationReportFormat.Table)]
    [TestCase (ViolationReportFormat.Xml)]
    [TestCase (ViolationReportFormat.VisualStudio)]
    public void Run_WithViolationsWithTableReportToConsole_CreatesAndCallsConsoleViolationReporter (ViolationReportFormat expectedReportFormat)
    {
      var expectedViolations = SetupAndReturnSomeViolations();

      // ACT
      var result = RunCommand(_sut, $"--reportFormat={expectedReportFormat}", "solution");

      // ASSERT
      result.Should().Be(1);

      AssertConfigurationLoadWithDefaultParameters();
      AssertCorrectCommandExecution();
      AssertCreatesAndCallsConsoleViolationReporterWithExpectedFormat(expectedReportFormat, expectedViolations);
    }

    [Test]
    [TestCase (ViolationReportFormat.Table)]
    [TestCase (ViolationReportFormat.Xml)]
    [TestCase (ViolationReportFormat.VisualStudio)]
    public void Run_WithViolationsWithReportToFile_CreatesAndCallsFileViolationReporter (ViolationReportFormat expectedReportFormat)
    {
      var expectedFilePath = "SomeFile.log";
      var expectedViolations = SetupAndReturnSomeViolations();

      // ACT
      var result = RunCommand(_sut, $"--reportFormat={expectedReportFormat}", $"--reportOutputFile={expectedFilePath}", "solution");

      // ASSERT
      result.Should().Be(1);

      AssertConfigurationLoadWithDefaultParameters();
      AssertCorrectCommandExecution();
      AssertCreatesAndCallsFileViolationReporterWithExpectedFormat(expectedReportFormat, expectedFilePath, expectedViolations);
    }

    [Test]
    public void Run_WithInvalidReportFormat_ReportsErrorAndAbortsCommand ()
    {
      // ACT
      var result = RunCommand(_sut, "--reportFormat=DOES_NOT_EXIST", "solution");

      // ASSERT
      result.Should().Be(-1);

      CapturedOutput.ToString().Should().Contain("Could not convert string `DOES_NOT_EXIST' to type ViolationReportFormat");
    }

    [Test]
    public void Run_WithNonExistingSolution_ReportsErrorAndAbortsCommand ()
    {
      A.CallTo(() => _solutionLoader.Load(A<string>._, A<IMsBuildParsingConfiguration>._))
          .Throws(new SolutionNotFoundException("DOES_NOT_EXIST"));

      // ACT
      var result = RunCommand(_sut, "DOES_NOT_EXIST");

      // ASSERT
      result.Should().Be(-1);

      A.CallTo(() => _configurationLoader.LoadRulesConfig($"DOES_NOT_EXIST.{SolutionInspector.RulesetFileExtension}")).MustHaveHappened();
      CapturedOutput.ToString().Should().Contain("Given solution file 'DOES_NOT_EXIST' could not be found.");

      AssertDoesNotCreateViolationReporter();
    }

    [Test]
    public void Run_WithSolutionContainingInvalidProject_ReportsErrorAndAbortsCommand ()
    {
      A.CallTo(() => _solutionLoader.Load(A<string>._, A<IMsBuildParsingConfiguration>._))
          .Throws(new InvalidProjectFileException("projectFile", 0, 0, 0, 0, "message", null, null, null));

      // ACT
      var result = RunCommand(_sut, "solution");

      // ASSERT
      result.Should().Be(-1);

      CapturedOutput.ToString()
          .Should()
          .Contain($"Given solution file 'solution' contains an invalid project file '{Environment.CurrentDirectory}\\projectFile'");

      AssertDoesNotCreateViolationReporter();
      AssertConfigurationLoadWithDefaultParameters();
    }

    [Test]
    public void Run_AndSolutionLoaderThrowsUnexpectedException_ReportsErrorAndAbortsCommand ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _solutionLoader.Load(A<string>._, A<IMsBuildParsingConfiguration>._)).Throws(thrownException);

      // ACT
      var result = RunCommand(_sut, "solution");

      // ASSERT
      result.Should().Be(-1);

      CapturedOutput.ToString().Should().Contain($"Unexpected error when loading solution file 'solution': {thrownException.Message}");

      AssertDoesNotCreateViolationReporter();
      AssertConfigurationLoadWithDefaultParameters();
    }

    private IReadOnlyCollection<IRuleViolation> SetupAndReturnSomeViolations ()
    {
      var solutionRuleViolation = new RuleViolation(_solutionRule, _solution, Some.String);
      A.CallTo(() => _solutionRule.Evaluate(A<ISolution>._)).Returns(new[] { solutionRuleViolation });

      var projectRuleViolation = new RuleViolation(_projectRule, _project, Some.String);
      A.CallTo(() => _projectRule.Evaluate(A<IProject>._)).Returns(new[] { projectRuleViolation });

      var projectItemRuleViolation = new RuleViolation(_projectItemRule, _projectItem, Some.String);
      A.CallTo(() => _projectItemRule.Evaluate(A<IProjectItem>._)).Returns(new[] { projectItemRuleViolation });

      return new[] { solutionRuleViolation, projectRuleViolation, projectItemRuleViolation };
    }

    private void AssertConfigurationLoadWithDefaultParameters ()
    {
      A.CallTo(() => _configurationLoader.LoadRulesConfig($"solution.{SolutionInspector.RulesetFileExtension}")).MustHaveHappened();
    }

    private void AssertCorrectCommandExecution ()
    {
      A.CallTo(() => _ruleAssemblyLoader.LoadRuleAssemblies(_ruleset.RuleAssemblyImports)).MustHaveHappened();
      A.CallTo(() => _solutionLoader.Load("solution", _solutionInspectorConfiguration.MsBuildParsing)).MustHaveHappened();

      A.CallTo(() => _ruleCollectionBuilder.Build(_rulesConfiguration)).MustHaveHappened();

      A.CallTo(() => _solutionRule.Evaluate(_solution)).MustHaveHappened(Repeated.Exactly.Once);
      A.CallTo(() => _projectRule.Evaluate(_project)).MustHaveHappened(Repeated.Exactly.Once);
      A.CallTo(() => _projectItemRule.Evaluate(_projectItem)).MustHaveHappened(Repeated.Exactly.Once);
    }

    private void AssertDoesNotCreateViolationReporter ()
    {
      A.CallTo(_violationReporterFactory).MustNotHaveHappened();
    }

    private void AssertCreatesAndCallsFileViolationReporterWithExpectedFormat (
      ViolationReportFormat expectedReportFormat,
      string expectedFilePath,
      IEnumerable<IRuleViolation> expectedViolations)
    {
      A.CallTo(() => _violationReporterFactory.CreateFileReporter(expectedReportFormat, expectedFilePath)).MustHaveHappened();
      A.CallTo(() => _violationReporter.Report(A<IEnumerable<IRuleViolation>>.That.IsSameSequenceAs(expectedViolations))).MustHaveHappened();
    }

    private void AssertCreatesAndCallsConsoleViolationReporterWithExpectedFormat (
      ViolationReportFormat expectedReportFormat,
      IEnumerable<IRuleViolation> expectedViolations)
    {
      A.CallTo(() => _violationReporterFactory.CreateConsoleReporter(expectedReportFormat)).MustHaveHappened();
      A.CallTo(() => _violationReporter.Report(A<IEnumerable<IRuleViolation>>.That.IsSameSequenceAs(expectedViolations))).MustHaveHappened();
    }
  }
}