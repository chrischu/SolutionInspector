using System;
using System.Collections.Generic;
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
  [Subject (typeof(AllProjectReferencesMustBeValidRule))]
  class AllProjectReferencesMustBeValidRuleSpec
  {
    static IProject Project;
    static IAdvancedProject AdvancedProject;

    static AllProjectReferencesMustBeValidRule SUT;

    Establish ctx = () =>
    {
      Project = A.Fake<IProject>();

      AdvancedProject = A.Fake<IAdvancedProject>();
      A.CallTo(() => Project.Advanced).Returns(AdvancedProject);

      SUT = new AllProjectReferencesMustBeValidRule();
    };

    class when_evaluating_project_reference_with_non_existing_project_file
    {
      // TODO 
    }
  }
}