using System;
using System.Configuration;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.TestInfrastructure.Configuration;

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

namespace SolutionInspector.Api.Tests.Configuration.MsBuildParsing
{
  [Subject(typeof (MsBuildParsingConfigurationSection))]
  class MsBuildParsingConfigurationSectionSpec
  {
    static IMsBuildParsingConfiguration SUT;

    Establish ctx = () =>
    {
      SUT = new MsBuildParsingConfigurationSection();
    };

    class when_deserializing_config
    {
      Because of = () => ConfigurationHelper.DeserializeSection((ConfigurationSection) SUT, MsBuildParsingConfigurationSection.ExampleConfiguration);

      It reads_project_build_actions = () =>
          SUT.ProjectBuildActions.Should().BeEquivalentTo(@"None");
    }
  }
}