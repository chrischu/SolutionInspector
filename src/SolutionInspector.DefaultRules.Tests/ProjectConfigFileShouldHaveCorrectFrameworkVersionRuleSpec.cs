using System;
using System.Collections.Generic;
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
  [Subject (typeof (ProjectConfigFileShouldHaveCorrectFrameworkVersionRule))]
  class ProjectConfigFileShouldHaveCorrectFrameworkVersionRuleSpec
  {
    static IProject Project;
    static IConfigurationProjectItem ConfigurationProjectItem;

    static ProjectConfigFileShouldHaveCorrectFrameworkVersionRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      ConfigurationProjectItem = A.Fake<IConfigurationProjectItem>();
      A.CallTo(() => Project.ConfigurationProjectItem).Returns(ConfigurationProjectItem);

      SUT = new ProjectConfigFileShouldHaveCorrectFrameworkVersionRule(
          new ProjectConfigurationFileShouldHaveCorrectFrameworkVersionRuleConfiguration
          {
              ExpectedVersion = "Version",
              ExpectedSKU = "SKU"
          });
    };

    class when_evaluating_project_without_framework_config
    {
      Establish ctx = () =>
      {
        var configurationXml =
            XDocument.Parse(@"<configuration></configuration>");

        A.CallTo(() => ConfigurationProjectItem.ConfigurationXml).Returns(configurationXml);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violation = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  ConfigurationProjectItem,
                  "No explicit configuration for the supported runtime version/SKU could be found."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_with_correct_framework_config
    {
      Establish ctx = () =>
      {
        var configurationXml =
            XDocument.Parse(@"<configuration><startup><supportedRuntime version=""Version"" sku=""SKU"" /></startup></configuration>");

        A.CallTo(() => ConfigurationProjectItem.ConfigurationXml).Returns(configurationXml);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It does_not_return_violations = () =>
          Result.Should().BeEmpty();

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_with_incorrect_framework_config
    {
      Establish ctx = () =>
      {
        var configurationXml =
            XDocument.Parse(
                @"<configuration><startup><supportedRuntime version=""DifferentVersion"" sku=""DifferentSKU"" /></startup></configuration>");

        A.CallTo(() => ConfigurationProjectItem.ConfigurationXml).Returns(configurationXml);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violations = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  ConfigurationProjectItem,
                  "Unexpected value for supported runtime version, was 'DifferentVersion' but should be 'Version'."),
              new RuleViolation(
                  SUT,
                  ConfigurationProjectItem,
                  "Unexpected value for supported runtime SKU, was 'DifferentSKU' but should be 'SKU'."));

      static IEnumerable<IRuleViolation> Result;
    }

    class when_evaluating_project_with_framework_config_without_sku_and_version
    {
      Establish ctx = () =>
      {
        var configurationXml =
            XDocument.Parse(
                @"<configuration><startup><supportedRuntime /></startup></configuration>");

        A.CallTo(() => ConfigurationProjectItem.ConfigurationXml).Returns(configurationXml);
      };

      Because of = () => Result = SUT.Evaluate(Project);

      It returns_violations = () =>
          Result.ShouldAllBeEquivalentTo(
              new RuleViolation(
                  SUT,
                  ConfigurationProjectItem,
                  "Unexpected value for supported runtime version, was '' but should be 'Version'."),
              new RuleViolation(
                  SUT,
                  ConfigurationProjectItem,
                  "Unexpected value for supported runtime SKU, was '' but should be 'SKU'."));

      static IEnumerable<IRuleViolation> Result;
    }
  }
}