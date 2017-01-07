using System.Collections;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Tests.ObjectModel
{
  public class ProjectItemBuildActionTests
  {
    [Test]
    [TestCaseSource(nameof(EqualsTestData))]
    public bool Equals(ProjectItemBuildAction a, [CanBeNull] ProjectItemBuildAction b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsTestData()
    {
      var buildAction = ProjectItemBuildAction.Custom("A");
      var buildActionClone = ProjectItemBuildAction.Custom("A");

      yield return new TestCaseData(buildAction, buildAction) { ExpectedResult = true, TestName = "Equals_ReferenceEquality" };
      yield return new TestCaseData(buildAction, buildActionClone) { ExpectedResult = true };

      yield return new TestCaseData(buildAction, ProjectItemBuildAction.Custom("X")) { ExpectedResult = false };
      yield return new TestCaseData(buildAction, null) { ExpectedResult = false };
    }

    [Test]
    [TestCaseSource(nameof(EqualsWithObjectsTestData))]
    public bool Equals_WithObjects(object a, [CanBeNull] object b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsWithObjectsTestData()
    {
      var buildAction = ProjectItemBuildAction.Custom("A");

      yield return new TestCaseData(buildAction, default(object)) { ExpectedResult = false };
      yield return new TestCaseData(buildAction, buildAction) { ExpectedResult = true };
    }

    [Test]
    public void EqualityOperators()
    {
      var a = ProjectItemBuildAction.Custom("A");
      var aClone = ProjectItemBuildAction.Custom("A");
      var b = ProjectItemBuildAction.Custom("B");

      // ACT & ASSERT
      (a == aClone).Should().BeTrue();
      (a != aClone).Should().BeFalse();

      (a == b).Should().BeFalse();
      (a != b).Should().BeTrue();
    }
  }
}