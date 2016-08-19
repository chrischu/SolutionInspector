using System.Xml.Linq;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Configuration;

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

namespace SolutionInspector.Api.Configuration.Tests.MsBuildParsing
{
  [Subject (typeof(MsBuildParsingConfigurationSection))]
  class MsBuildParsingConfigurationSectionSpec
  {
    static IMsBuildParsingConfiguration SUT;

    class when_deserializing_config
    {
      Establish ctx = () =>
      {
        Element = XDocument.Parse(MsBuildParsingConfigurationSection.ExampleConfiguration).Root;
      };

      Because of = () => SUT = ConfigurationElement.Load<MsBuildParsingConfigurationSection>(Element);

      It reads_project_build_actions = () =>
      {
        SUT.IsValidProjectItemType("None").Should().BeTrue();
        SUT.IsValidProjectItemType("Something").Should().BeFalse();
      };

      static XElement Element;
    }
  }
}