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
    static IViolationReporterProxy ViolationReporterProxy;

    static ISolutionInspectorConfiguration Configuration;
    static IRulesConfiguration RulesConfiguration;

    static ISolutionRule SolutionRule;
    static IProjectRule ProjectRule;
    static IProjectItemRule ProjectItemRule;

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

      ViolationReporterProxy = A.Fake<IViolationReporterProxy>();

      SUT = new InspectCommand(Configuration, SolutionLoader, RuleCollectionBuilder, ViolationReporterProxy);
    };

    class when_running_without_violations
    {
      Because of = () => Result = RunCommand(SUT, "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_does_not_call_the_violation_reporter> __;

      It returns_exit_code = () =>
          Result.Should().Be(0);

      protected static string ExpectedConfigurationFile;
      static int Result;
    }

    class when_running_with_violations_without_specifying_report
    {
      Establish ctx = () =>
      {
        A.CallTo(() => SolutionRule.Evaluate(A<ISolution>._)).Returns(new[] { new RuleViolation(SolutionRule, Solution, Some.String()) });
        A.CallTo(() => ProjectRule.Evaluate(A<IProject>._)).Returns(new[] { new RuleViolation(ProjectRule, Project, Some.String()) });
      };

      Because of = () => Result = RunCommand(SUT, "solution");

      Behaves_like<it_executes_the_command_correctly> _;
      Behaves_like<it_does_not_call_the_violation_reporter> __;

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      static int Result;
    }

    class when_running_with_violations_with_table_report
    {
      Establish ctx = () =>
      {
        SolutionRuleViolation = new RuleViolation(SolutionRule, Solution, Some.String());
        A.CallTo(() => SolutionRule.Evaluate(A<ISolution>._)).Returns(new[] { SolutionRuleViolation });

        ProjectRuleViolation = new RuleViolation(ProjectRule, Project, Some.String());
        A.CallTo(() => ProjectRule.Evaluate(A<IProject>._)).Returns(new[] { ProjectRuleViolation });
      };

      Because of = () => Result = RunCommand(SUT, "--report=Table", "solution");

      Behaves_like<it_executes_the_command_correctly> _;

      It calls_the_violation_reporter = () =>
          A.CallTo(
              () =>
                  ViolationReporterProxy.Report(
                      ViolationReportFormat.Table,
                      A<IEnumerable<IRuleViolation>>.That.IsSameSequenceAs(new[] { SolutionRuleViolation, ProjectRuleViolation })))
              .MustHaveHappened();

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      static RuleViolation SolutionRuleViolation;
      static RuleViolation ProjectRuleViolation;
      static int Result;
    }

    class when_running_with_violations_with_xml_report
    {
      Establish ctx = () =>
      {
        SolutionRuleViolation = new RuleViolation(SolutionRule, Solution, Some.String());
        A.CallTo(() => SolutionRule.Evaluate(A<ISolution>._)).Returns(new[] { SolutionRuleViolation });

        ProjectRuleViolation = new RuleViolation(ProjectRule, Project, Some.String());
        A.CallTo(() => ProjectRule.Evaluate(A<IProject>._)).Returns(new[] { ProjectRuleViolation });
      };

      Because of = () => Result = RunCommand(SUT, "--report=Xml", "solution");

      Behaves_like<it_executes_the_command_correctly> _;

      It calls_the_violation_reporter = () =>
          A.CallTo(
              () =>
                  ViolationReporterProxy.Report(
                      ViolationReportFormat.Xml,
                      A<IEnumerable<IRuleViolation>>.That.IsSameSequenceAs(new[] { SolutionRuleViolation, ProjectRuleViolation })))
              .MustHaveHappened();

      It returns_exit_code = () =>
          Result.Should().Be(1);

      protected static string ExpectedConfigurationFile;
      static RuleViolation SolutionRuleViolation;
      static RuleViolation ProjectRuleViolation;
      static int Result;
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
    class it_does_not_call_the_violation_reporter
    {
      It does_not_call_the_violation_reporter = () =>
          A.CallTo(() => ViolationReporterProxy.Report(A<ViolationReportFormat>._, A<IEnumerable<RuleViolation>>._)).MustNotHaveHappened();
    }

    static int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, TextWriter.Null);
    }
  }
}