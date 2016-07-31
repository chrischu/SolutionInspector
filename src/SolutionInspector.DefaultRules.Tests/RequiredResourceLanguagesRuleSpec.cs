using System;
using System.Collections.Generic;
using System.Configuration;
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
  [Subject (typeof(RequiredResourceLanguagesRule))]
  class RequiredResourceLanguagesRuleSpec
  {
    static IProject Project;

    static RequiredResourceLanguagesRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      SUT = new RequiredResourceLanguagesRule(
          new RequiredResourceLanguagesRuleConfiguration
          {
              RequiredResources = new CommaDelimitedStringCollection { "Resources1", "Resources2" },
              RequiredLanguages = new CommaDelimitedStringCollection { "de", "cs" }
          });
    };

    class when_project_contains_resources_for_all_required_languages
    {
      Establish ctx = () =>
      {
        A.CallTo(() => Project.ProjectItems).Returns(
            new[]
            {
                CreateProjectItem("Resources1.resx"),
                CreateProjectItem("Resources1.de.resx"),
                CreateProjectItem("Resources1.cs.resx"),
                CreateProjectItem("Resources2.resx"),
                CreateProjectItem("Resources2.de.resx"),
                CreateProjectItem("Resources2.cs.resx")
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_project_does_not_contain_default_resource_file_for_required_resources
    {
      Establish ctx = () =>
      {
        A.CallTo(() => Project.ProjectItems).Returns(
            new[]
            {
                CreateProjectItem("Resources1.de.resx"),
                CreateProjectItem("Resources1.cs.resx"),
                CreateProjectItem("Resources2.de.resx"),
                CreateProjectItem("Resources2.cs.resx")
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(SUT, Project, "For the required resource 'Resources1' no default resource file ('Resources1.resx') could be found."),
              new RuleViolation(SUT, Project, "For the required resource 'Resources2' no default resource file ('Resources2.resx') could be found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_project_does_not_contain_resource_file_for_a_required_language
    {
      Establish ctx = () =>
      {
        A.CallTo(() => Project.ProjectItems).Returns(
            new[]
            {
                CreateProjectItem("Resources1.resx"),
                CreateProjectItem("Resources1.cs.resx"),
                CreateProjectItem("Resources2.resx"),
                CreateProjectItem("Resources2.cs.resx")
            });
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeLike(
              new RuleViolation(SUT, Project, "For the required resource 'Resources1' no resource file for language 'de' ('Resources1.de.resx') could be found."),
              new RuleViolation(SUT, Project, "For the required resource 'Resources2' no resource file for language 'de' ('Resources2.de.resx') could be found."));

      static IEnumerable<IRuleViolation> Result;
    }

    static IProjectItem CreateProjectItem(string name)
    {
      var projectItem = A.Fake<IProjectItem>();
      A.CallTo(() => projectItem.Name).Returns(name);
      return projectItem;
    }
  }
}