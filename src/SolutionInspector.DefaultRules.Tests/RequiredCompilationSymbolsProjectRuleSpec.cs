using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using SolutionInspector.Configuration;
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
  [Subject (typeof(RequiredCompilationSymbolsProjectRule))]
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

      var requiredCompilationSymbolsProjectRuleConfiguration = ConfigurationElement.Create<RequiredCompilationSymbolsProjectRuleConfiguration>(
        initialize: e =>
        {
          var item = e.RequiredCompilationSymbols.AddNew();
          item.BuildConfigurationFilter = new BuildConfigurationFilter(new BuildConfiguration("Included", "*"));
          item.RequiredCompilationSymbols.Add("TRACE", "DEBUG");
        });
      SUT = new RequiredCompilationSymbolsProjectRule(requiredCompilationSymbolsProjectRuleConfiguration);
    };

    class when_all_required_symbols_are_there
    {
      Establish ctx = () =>
      {
        var property = A.Fake<IEvaluatedProjectPropertyValue>();
        A.CallTo(() => property.Value).Returns("TRACE;DEBUG");
        A.CallTo(() => AdvancedProject.EvaluateProperties(IncludedBuildConfiguration, null))
            .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property } });
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
        var property = A.Fake<IEvaluatedProjectPropertyValue>();
        A.CallTo(() => property.Value).Returns("TRACE");
        A.CallTo(() => AdvancedProject.EvaluateProperties(IncludedBuildConfiguration, null))
            .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property } });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(
                  SUT,
                  Project,
                  $"In the build configuration '{IncludedBuildConfiguration}' the required compilation symbol 'DEBUG' was not found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_a_required_symbol_is_missing_in_a_filtered_configuration
    {
      Establish ctx = () =>
      {
        var property = A.Fake<IEvaluatedProjectPropertyValue>();
        A.CallTo(() => property.Value).Returns("TRACE;DEBUG");
        A.CallTo(() => AdvancedProject.EvaluateProperties(IncludedBuildConfiguration, null))
            .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property } });

        var property2 = A.Fake<IEvaluatedProjectPropertyValue>();
        A.CallTo(() => property2.Value).Returns("TRACE");
        A.CallTo(() => AdvancedProject.EvaluateProperties(FilteredBuildConfiguration, null))
            .Returns(new Dictionary<string, IEvaluatedProjectPropertyValue> { { "DefineConstants", property2 } });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }
  }
}