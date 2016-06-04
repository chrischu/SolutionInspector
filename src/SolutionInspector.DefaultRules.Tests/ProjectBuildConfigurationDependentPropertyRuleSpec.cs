using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
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

namespace SolutionInspector.DefaultRules.Tests
{
  [Subject (typeof (ProjectBuildConfigurationDependentPropertyRule))]
  class ProjectBuildConfigurationDependentPropertyRuleSpec
  {
    static IProject Project;
    static IAdvancedProject AdvancedProject;

    static string Property;
    static string ExpectedValue;

    static BuildConfiguration BuildConfiguration1;
    static BuildConfiguration BuildConfiguration2;
    static BuildConfiguration BuildConfiguration3;

    static BuildConfigurationFilter FilterIncludeOneOnly;
    static BuildConfigurationFilter FilterIncludeOneAndTwo;

    static ProjectBuildConfigurationDependentPropertyRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      BuildConfiguration1 = new BuildConfiguration("Configuration", "Platform");
      BuildConfiguration2 = new BuildConfiguration("Configuration", "Platform2");
      BuildConfiguration3 = new BuildConfiguration("Configuration2", "Platform");

      FilterIncludeOneOnly = new BuildConfigurationFilter(BuildConfiguration1);
      FilterIncludeOneAndTwo = new BuildConfigurationFilter(new BuildConfiguration("Configuration", "*"));

      AdvancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => Project.Advanced).Returns(AdvancedProject);
      A.CallTo(() => Project.BuildConfigurations).Returns(new[] { BuildConfiguration1, BuildConfiguration2, BuildConfiguration3 });

      Property = "Property";
      ExpectedValue = "ExpectedValue";

      SUT = new ProjectBuildConfigurationDependentPropertyRule(
          new ProjectBuildConfigurationDependentPropertyRuleConfiguration
          {
              Property = Property,
              ExpectedValue = ExpectedValue,
              BuildConfigurationFilter = FilterIncludeOneOnly
          });
    };

    class when_evaluating_property_with_same_value
    {
      Establish ctx = () =>
      {
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration1, Property, ExpectedValue);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_property_with_different_value
    {
      Establish ctx = () =>
      {
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration1, Property, "ActualValue");
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  Project,
                  "Unexpected value for property 'Property' in build configuration " +
                  $"'{BuildConfiguration1}', was 'ActualValue' but should be 'ExpectedValue'."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_non_existing_property
    {
      Establish ctx = () =>
      {
        A.CallTo(() => AdvancedProject.EvaluateProperties(BuildConfiguration1, null))
            .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue>());
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  Project,
                  "Unexpected value for property 'Property' in build configuration " +
                  $"'{BuildConfiguration1}', was '<null>' but should be 'ExpectedValue'."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_property_that_differs_in_other_configuration
    {
      Establish ctx = () =>
      {
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration1, Property, ExpectedValue);
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration2, Property, "ActualValue");
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_no_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_properties_that_differ_in_some_configurations
    {
      Establish ctx = () =>
      {
        SUT = new ProjectBuildConfigurationDependentPropertyRule(
            new ProjectBuildConfigurationDependentPropertyRuleConfiguration
            {
                Property = Property,
                ExpectedValue = ExpectedValue,
                BuildConfigurationFilter = FilterIncludeOneAndTwo
            });

        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration1, Property, "ActualValue");
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration2, Property, "ActualValue");
        ProjectPropertyFakeUtility.SetupFakeBuildConfigurationDependentProperty(AdvancedProject, BuildConfiguration3, Property, ExpectedValue);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violations_from_evaluated_configurations = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  Project,
                  "Unexpected value for property 'Property' in build configuration " +
                  $"'{BuildConfiguration1}', was 'ActualValue' but should be 'ExpectedValue'."),
              new RuleViolation(
                  SUT,
                  Project,
                  "Unexpected value for property 'Property' in build configuration " +
                  $"'{BuildConfiguration2}', was 'ActualValue' but should be 'ExpectedValue'."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}