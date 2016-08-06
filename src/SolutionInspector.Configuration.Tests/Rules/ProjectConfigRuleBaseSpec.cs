using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Configuration.Tests.Rules
{
  [Subject (typeof(ProjectConfigRuleBase<>))]
  class ProjectConfigRuleBaseSpec
  {
    static DummyProjectConfigRule SUT;

    static DummyProjectConfigurationRuleConfiguration Configuration;
    static IRuleViolation RuleViolation;

    static IProject Project;

    Establish ctx = () =>
    {
      Configuration = new DummyProjectConfigurationRuleConfiguration();
      RuleViolation = A.Fake<IRuleViolation>();

      SUT = new DummyProjectConfigRule(Configuration, RuleViolation);

      Project = A.Fake<IProject>();
      A.CallTo(() => Project.Name).Returns("Project");
    };


    class when_evaluating
    {
      Because of =
          () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.Single().Should().BeSameAs(RuleViolation);

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_a_project_without_configuration_and_reportViolationOnMissingConfigurationFile_is_true
    {
      Establish ctx = () =>
      {
        Configuration.ReportViolationOnMissingConfigurationFile = true;
        A.CallTo(() => Project.ConfigurationProjectItem).Returns(null);
      };

      Because of =
          () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(new RuleViolation(SUT, Project, "For the project 'Project' no configuration file could be found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_a_project_without_configuration_and_reportViolationOnMissingConfigurationFile_is_false
    {
      Establish ctx = () =>
      {
        Configuration.ReportViolationOnMissingConfigurationFile = false;
        A.CallTo(() => Project.ConfigurationProjectItem).Returns(null);
      };

      Because of =
          () => Result = SUT.Evaluate(Project);

      It returns_no_violation = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class DummyProjectConfigRule : ProjectConfigRuleBase<DummyProjectConfigurationRuleConfiguration>
    {
      private readonly IRuleViolation _violation;

      public DummyProjectConfigRule (DummyProjectConfigurationRuleConfiguration configuration, IRuleViolation violation)
          : base(configuration)
      {
        _violation = violation;
      }

      protected override IEnumerable<IRuleViolation> Evaluate (
          IConfigurationProjectItem target,
          XDocument configurationXml)
      {
        yield return _violation;
      }
    }

    class DummyProjectConfigurationRuleConfiguration : ProjectConfigRuleConfigurationBase
    {
      public DummyProjectConfigurationRuleConfiguration ()
      {
        ReportViolationOnMissingConfigurationFile = true;
      }
    }
  }
}