using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.TestInfrastructure;

#region R# preamble for Machine.Specifications files

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

namespace SolutionInspector.Api.Tests.ObjectModel
{
  [Subject (typeof(ProjectItemBuildAction))]
  class ProjectItemBuildActionSpec
  {
    class when_comparing_equality
    {
      Establish ctx = () =>
      {
        var value = Some.String();
        A = ProjectItemBuildAction.Custom(value);
        EqualToA = ProjectItemBuildAction.Custom(value);
      };

      Because of = () =>
      {
        /* Actual tests are in the its. */
      };

      It works_with_same_reference = () =>
          A.Equals(A).Should().BeTrue();

      It works_with_null = () =>
          A.Equals(null).Should().BeFalse();

      It works_with_equal_instances = () =>
          A.Equals(EqualToA).Should().BeTrue();

      It works_with_differing_instances = () =>
          A.Equals(ProjectItemBuildAction.Custom("DIFFERENT")).Should().BeFalse();

      It works_with_same_reference_as_object = () =>
          A.Equals((object) A).Should().BeTrue();

      It works_with_null_as_object = () =>
          A.Equals((object) null).Should().BeFalse();

      It works_with_equality_operator = () =>
          (A == EqualToA).Should().BeTrue();

      It works_with_inequality_operator = () =>
          (A != EqualToA).Should().BeFalse();

      static ProjectItemBuildAction A;
      static ProjectItemBuildAction EqualToA;
    }
  }
}