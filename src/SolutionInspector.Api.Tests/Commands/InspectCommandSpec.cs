using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using ManyConsole;
using SolutionInspector.Api.Commands;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Configuration.Rules;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;
using SolutionInspector.TestInfrastructure;

#region R# preamble for Machine.Specifications files

#pragma warning disable 414

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

namespace SolutionInspector.Api.Tests.Commands
{
  [Subject (typeof(InspectCommand))]
  class InspectCommandSpec
  {
    static ISolution Solution;
    static IProject Project;
    static IProjectItem ProjectItem;
    static IRuleCollection Rules;

    static ISolutionLoader SolutionLoader;
    static IRuleCollectionBuilder RuleCollectionBuilder;
    static IViolationReporter ViolationReporter;
    static IViolationReporterFactory ViolationReporterFactory;

    static ISolutionInspectorConfiguration Configuration;
    static IRulesConfiguration RulesConfiguration;

    static ISolutionRule SolutionRule;
    static IProjectRule ProjectRule;
    static IProjectItemRule ProjectItemRule;

    static TextWriter TextWriter;

    static InspectCommand SUT;

    Establish ctx = () =>
    {
      Solution = A.Fake<ISolution>();

      Project = A.Fake<IProject>();
      A.CallTo(() => Solution.Projects).Returns(new[] { Project });

      ProjectItem = A.Fake<IProjectItem>();
      A.CallTo(() => Project.ProjectItems).Returns(new[] { ProjectItem });

      SolutionLoader = A.Fake<ISolutionLoader>();
      A.CallTo(() => SolutionLoader.Load(A<string>._, A<IMsBuildParsingConfiguration>._)).Returns(Solution);

      RuleCollectionBuilder = A.Fake<IRuleCollectionBuilder>();
      SolutionRule = A.Fake<ISolutionRule>();
      ProjectRule = A.Fake<IProjectRule>();
      ProjectItemRule = A.Fake<IProjectItemRule>();
      Rules = new RuleCollection(new[] { SolutionRule }, new[] { ProjectRule }, new[] { ProjectItemRule });
      A.CallTo(() => RuleCollectionBuilder.Build(A<IRulesConfiguration>._)).Returns(Rules);

      Configuration = A.Fake<ISolutionInspectorConfiguration>();
      RulesConfiguration = A.Fake<IRulesConfiguration>();
      A.CallTo(() => Configuration.Rules).Returns(RulesConfiguration);

      ViolationReporterFactory = A.Fake<IViolationReporterFactory>();

      ViolationReporter = A.Fake<IViolationReporter>();
      A.CallTo(() => ViolationReporterFactory.CreateConsoleReporter(A<ViolationReportFormat>._)).Returns(ViolationReporter);
      A.CallTo(() => ViolationReporterFactory.CreateFileReporter(A<ViolationReportFormat>._, A<string>._)).Returns(ViolationReporter);

      TextWriter = new StringWriter();

      SUT = new InspectCommand(Configuration, SolutionLoader, RuleCollectionBuilder, ViolationReporterFactory);
    };

    class when_running_without_violations
    {
      Because of = () => Result = RunCommand(SUT, "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_does_not_create_a_violation_reporter> __;

      It returns_exit_code = () =>
          Result.Should().Be(0);

      protected static string ExpectedConfigurationFile;
      static int Result;
    }

    class when_running_with_violations_without_specifying_report
    {
      Establish ctx = () =>
      {
        ExpectedReportFormat = ViolationReportFormat.Xml;
        ExpectedViolations = SetupSomeViolations();
      };

      Because of = () => Result = RunCommand(SUT, "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_creates_and_calls_a_console_violation_reporter_with_the_expected_format> __;

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      protected static ViolationReportFormat ExpectedReportFormat;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
      static int Result;
    }

    class when_running_with_violations_with_table_report_to_console
    {
      Establish ctx = () =>
      {
        ExpectedReportFormat = ViolationReportFormat.Table;
        ExpectedViolations = SetupSomeViolations();
      };

      Because of = () => Result = RunCommand(SUT, $"--reportFormat={ExpectedReportFormat}", "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_creates_and_calls_a_console_violation_reporter_with_the_expected_format> __;

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      protected static ViolationReportFormat ExpectedReportFormat;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
      static int Result;
    }

    class when_running_with_violations_with_table_report_to_file
    {
      Establish ctx = () =>
      {
        ExpectedReportFormat = ViolationReportFormat.Table;
        ExpectedFilePath = "SomeFile.log";
        ExpectedViolations = SetupSomeViolations();
      };

      Because of = () => Result = RunCommand(SUT, $"--reportFormat={ExpectedReportFormat}", $"--reportOutputFile={ExpectedFilePath}", "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_creates_and_calls_a_file_violation_reporter_with_the_expected_format> __;

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      protected static ViolationReportFormat ExpectedReportFormat;
      protected static string ExpectedFilePath;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
      static int Result;
    }

    class when_running_with_violations_with_xml_report_to_console
    {
      Establish ctx = () =>
      {
        ExpectedReportFormat = ViolationReportFormat.Xml;
        ExpectedViolations = SetupSomeViolations();
      };

      Because of = () => Result = RunCommand(SUT, $"-f {ExpectedReportFormat}", "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_creates_and_calls_a_console_violation_reporter_with_the_expected_format> __;

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      protected static ViolationReportFormat ExpectedReportFormat;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
      static int Result;
    }

    class when_running_with_violations_with_xml_report_to_file
    {
      Establish ctx = () =>
      {
        ExpectedReportFormat = ViolationReportFormat.Xml;
        ExpectedFilePath = "SomeFile.log";
        ExpectedViolations = SetupSomeViolations();
      };

      Because of = () => Result = RunCommand(SUT, $"--reportFormat={ExpectedReportFormat}", $"-o {ExpectedFilePath}", "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_creates_and_calls_a_file_violation_reporter_with_the_expected_format> __;

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      protected static ViolationReportFormat ExpectedReportFormat;
      protected static string ExpectedFilePath;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
      static int Result;
    }

    class when_running_with_invalid_report_format
    {
      Because of = () => Result = RunCommand(SUT, "--reportFormat=DOES_NOT_EXIST");

      It shows_error = () =>
          TextWriter.ToString().Should().Contain("Could not convert string `DOES_NOT_EXIST' to type ViolationReportFormat");

      It returns_exit_code = () =>
          Result.Should().Be(-1);

      static int Result;
    }

    static IEnumerable<IRuleViolation> SetupSomeViolations ()
    {
      var solutionRuleViolation = new RuleViolation(SolutionRule, Solution, Some.String());
      A.CallTo(() => SolutionRule.Evaluate(A<ISolution>._)).Returns(new[] { solutionRuleViolation });

      var projectRuleViolation = new RuleViolation(ProjectRule, Project, Some.String());
      A.CallTo(() => ProjectRule.Evaluate(A<IProject>._)).Returns(new[] { projectRuleViolation });

      var projectItemRuleViolation = new RuleViolation(ProjectItemRule, ProjectItem, Some.String());
      A.CallTo(() => ProjectItemRule.Evaluate(A<IProjectItem>._)).Returns(new[] { projectItemRuleViolation });

      return new[] { solutionRuleViolation, projectRuleViolation, projectItemRuleViolation };
    }

    [Behaviors]
    class it_executes_the_command_correctly
    {
      It loads_solution = () =>
          A.CallTo(() => SolutionLoader.Load("solution", Configuration.MsBuildParsing)).MustHaveHappened();

      It builds_rules = () =>
          A.CallTo(() => RuleCollectionBuilder.Build(RulesConfiguration)).MustHaveHappened();

      It calls_solution_rule = () =>
          A.CallTo(() => SolutionRule.Evaluate(Solution)).MustHaveHappened(Repeated.Exactly.Once);

      It calls_project_rule = () =>
          A.CallTo(() => ProjectRule.Evaluate(Project)).MustHaveHappened(Repeated.Exactly.Once);

      It calls_project_item_rule = () =>
          A.CallTo(() => ProjectItemRule.Evaluate(ProjectItem)).MustHaveHappened(Repeated.Exactly.Once);

      protected static string ExpectedConfigurationFile;
    }

    [Behaviors]
    class it_does_not_create_a_violation_reporter
    {
      It does_not_create_a_violation_reporter = () => A.CallTo(ViolationReporterFactory).MustNotHaveHappened();
    }

    [Behaviors]
    class it_creates_and_calls_a_file_violation_reporter_with_the_expected_format
    {
      It creates_a_file_violation_reporter_with_the_expected_format = () =>
          A.CallTo(() => ViolationReporterFactory.CreateFileReporter(ExpectedReportFormat, ExpectedFilePath)).MustHaveHappened();

      It calls_the_file_violation_reporter = () =>
          A.CallTo(() => ViolationReporter.Report(A<IEnumerable<IRuleViolation>>.That.IsSameSequenceAs(ExpectedViolations))).MustHaveHappened();

      protected static ViolationReportFormat ExpectedReportFormat;
      protected static string ExpectedFilePath;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
    }

    [Behaviors]
    class it_creates_and_calls_a_console_violation_reporter_with_the_expected_format
    {
      It creates_a_file_violation_reporter_with_the_expected_format = () =>
          A.CallTo(() => ViolationReporterFactory.CreateConsoleReporter(ExpectedReportFormat)).MustHaveHappened();

      It calls_the_file_violation_reporter = () =>
          A.CallTo(() => ViolationReporter.Report(A<IEnumerable<IRuleViolation>>.That.IsSameSequenceAs(ExpectedViolations))).MustHaveHappened();

      protected static ViolationReportFormat ExpectedReportFormat;
      protected static IEnumerable<IRuleViolation> ExpectedViolations;
    }

    static int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, TextWriter);
    }
  }
}