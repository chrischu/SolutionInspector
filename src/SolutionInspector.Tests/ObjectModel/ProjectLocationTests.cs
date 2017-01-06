using System.Collections;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Tests.ObjectModel
{
  public class ProjectLocationTests
  {
    [Test]
    [TestCaseSource (nameof(EqualsTestData))]
    public bool Equals (ProjectLocation a, [CanBeNull] ProjectLocation b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsTestData ()
    {
      var a = new ProjectLocation(Some.PositiveInteger, Some.PositiveInteger);
      var equalToA = new ProjectLocation(a.Line, a.Column);
      var differentFromA = new ProjectLocation(Some.PositiveInteger, Some.PositiveInteger);

      yield return new TestCaseData(a, a) { ExpectedResult = true, TestName = "Equals_ReferenceEquality" };
      yield return new TestCaseData(a, equalToA) { ExpectedResult = true };
      yield return new TestCaseData(a, differentFromA) { ExpectedResult = false };

      yield return new TestCaseData(a, null) { ExpectedResult = false };
    }

    [Test]
    public void EqualityOperators ()
    {
      var a = new ProjectLocation(Some.PositiveInteger, Some.PositiveInteger);
      var equalToA = new ProjectLocation(a.Line, a.Column);
      var differentFromA = new ProjectLocation(Some.PositiveInteger, Some.PositiveInteger);

      // ACT & ASSERT
      (a == equalToA).Should().BeTrue();
      (a != equalToA).Should().BeFalse();

      (a == differentFromA).Should().BeFalse();
      (a != differentFromA).Should().BeTrue();
    }
  }
}