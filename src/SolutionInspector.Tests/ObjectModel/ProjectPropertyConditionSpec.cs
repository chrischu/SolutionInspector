using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.ObjectModel;
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

namespace SolutionInspector.Tests.ObjectModel
{
  [Subject (typeof(ProjectPropertyCondition))]
  class ProjectPropertyConditionSpec
  {
    class when_comparing_equality
    {
      Establish ctx = () =>
      {
        A = new ProjectPropertyCondition(Some.String(), Some.String());
        EqualToA = new ProjectPropertyCondition(A.Self, A.Parent);
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
          A.Equals(new ProjectPropertyCondition(A.Self, "DIFFERENT")).Should().BeFalse();

      It works_with_same_reference_as_object = () =>
          A.Equals((object) A).Should().BeTrue();

      It works_with_null_as_object = () =>
          A.Equals((object) null).Should().BeFalse();

      It works_with_equality_operator = () =>
          (A == EqualToA).Should().BeTrue();

      It works_with_inequality_operator = () =>
          (A != EqualToA).Should().BeFalse();

      static ProjectPropertyCondition A;
      static ProjectPropertyCondition EqualToA;
    }
  }
}