using System.Collections;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Tests.ObjectModel
{
  public class ProjectPropertyConditionTests
  {
    [Test]
    [TestCaseSource (nameof(EqualsTestData))]
    public bool Equals (ProjectPropertyCondition a, [CanBeNull] ProjectPropertyCondition b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsTestData ()
    {
      var a = new ProjectPropertyCondition(Some.String(), Some.String());
      var equalToA = new ProjectPropertyCondition(a.Self, a.Parent);
      var differentFromA = new ProjectPropertyCondition(Some.String(), Some.String());

      yield return new TestCaseData(a, a) { ExpectedResult = true, TestName = "Equals_ReferenceEquality" };
      yield return new TestCaseData(a, equalToA) { ExpectedResult = true };
      yield return new TestCaseData(a, differentFromA) { ExpectedResult = false };

      yield return new TestCaseData(a, null) { ExpectedResult = false };
    }

    [Test]
    public void EqualityOperators ()
    {
      var a = new ProjectPropertyCondition(Some.String(), Some.String());
      var equalToA = new ProjectPropertyCondition(a.Self, a.Parent);
      var differentFromA = new ProjectPropertyCondition(Some.String(), Some.String());

      // ACT & ASSERT
      (a == equalToA).Should().BeTrue();
      (a != equalToA).Should().BeFalse();

      (a == differentFromA).Should().BeFalse();
      (a != differentFromA).Should().BeTrue();
    }
  }
}