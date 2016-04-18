using System.Collections.Generic;
using System.Configuration;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.ObjectModel;
using SolutionInspector.Rules;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using SolutionInspector.Utilities;

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
  [Subject(typeof (RequiredCompilationSymbolsProjectRule))]
  class RequiredCompilationSymbolsProjectRuleSpec
  {
    static IProject Project;
    static IAdvancedProject AdvancedProject;

    static BuildConfiguration IncludedBuildConfiguration;
    static BuildConfiguration FilteredBuildConfiguration;


    static RequiredCompilationSymbolsProjectRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      IncludedBuildConfiguration = new BuildConfiguration("Included", "Platform");
      FilteredBuildConfiguration = new BuildConfiguration("Filtered", "Platform");

      AdvancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => Project.Advanced).Returns(AdvancedProject);
      A.CallTo(() => Project.BuildConfigurations).Returns(new[] { IncludedBuildConfiguration, FilteredBuildConfiguration });

      SUT = new RequiredCompilationSymbolsProjectRule(
          new RequiredCompilationSymbolsProjectRuleConfiguration
          {
              new RequiredCompilationSymbolsConfiguration
              {
                  BuildConfigurationFilter = new BuildConfigurationFilter(new BuildConfiguration("Included", "*")),
                  RequiredCompilationSymbols = new CommaDelimitedStringCollection { "TRACE", "DEBUG" }
              }
          }
          );
    };

    class when_all_required_symbols_are_there
    {
      Establish ctx = () =>
      {
        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[IncludedBuildConfiguration])
            .Returns(new Dictionary<string, string> { { "DefineConstants", "TRACE;DEBUG" } });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_a_required_symbol_is_missing
    {
      Establish ctx = () =>
      {
        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[IncludedBuildConfiguration])
            .Returns(new Dictionary<string, string> { { "DefineConstants", "TRACE" } });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  Project,
                  $"In the build configuration '{IncludedBuildConfiguration}' the required compilation symbol 'DEBUG' was not found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_all_required_symbol_is_missing_in_a_filtered_configuration
    {
      Establish ctx = () =>
      {
        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[IncludedBuildConfiguration])
            .Returns(new Dictionary<string, string> { { "DefineConstants", "TRACE;DEBUG" } });

        A.CallTo(() => AdvancedProject.ConfigurationDependentProperties[FilteredBuildConfiguration])
            .Returns(new Dictionary<string, string> { { "DefineConstants", "TRACE" } });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }
  }
}