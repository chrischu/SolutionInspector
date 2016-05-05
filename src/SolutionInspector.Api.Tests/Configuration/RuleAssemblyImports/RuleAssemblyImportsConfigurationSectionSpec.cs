using System;
using System.Configuration;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Configuration.RuleAssemblyImports;
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

namespace SolutionInspector.Api.Tests.Configuration.RuleAssemblyImports
{
  [Subject(typeof (RuleAssemblyImportsConfigurationSection))]
  class RuleAssemblyImportsConfigurationSectionSpec
  {
    static IRuleAssemblyImportsConfiguration SUT;

    Establish ctx = () =>
    {
      SUT = new RuleAssemblyImportsConfigurationSection();
    };

    class when_deserializing_config
    {
      Because of = () => ConfigurationHelper.DeserializeSection((ConfigurationSection) SUT, RuleAssemblyImportsConfigurationSection.ExampleConfiguration);

      It reads_rule_assembly_imports = () =>
          SUT.Imports.Should().BeEquivalentTo(@"C:\Path\To\Assembly.dll");
    }
  }
}