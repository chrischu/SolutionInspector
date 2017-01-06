using System.Collections;
using FluentAssertions;
using JetBrains.Annotations;
using Machine.Specifications;
using NUnit.Framework;
using SolutionInspector.ObjectModel;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Tests.ObjectModel
{
  public class ProjectItemIncludeTests
  {
    [Test]
    [TestCaseSource(nameof(EqualsTestData))]
    public bool Equals(ProjectItemInclude a, [CanBeNull] ProjectItemInclude b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsTestData()
    {
      var a = new ProjectItemInclude(Some.String(), Some.String());
      var equalToA = new ProjectItemInclude(a.Evaluated, a.Unevaluated);
      var differentFromA = new ProjectItemInclude(Some.String(), Some.String());

      yield return new TestCaseData(a, a) { ExpectedResult = true, TestName = "Equals_ReferenceEquality" };
      yield return new TestCaseData(a, equalToA) { ExpectedResult = true };
      yield return new TestCaseData(a, differentFromA) { ExpectedResult = false };

      yield return new TestCaseData(a, null) { ExpectedResult = false };
    }

    [Test]
    public void EqualityOperators()
    {
      var a = new ProjectItemInclude(Some.String(), Some.String());
      var equalToA = new ProjectItemInclude(a.Evaluated, a.Unevaluated);
      var differentFromA = new ProjectItemInclude(Some.String(), Some.String());

      // ACT & ASSERT
      (a == equalToA).Should().BeTrue();
      (a != equalToA).Should().BeFalse();

      (a == differentFromA).Should().BeFalse();
      (a != differentFromA).Should().BeTrue();
    }
  }
}