using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Utilities;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Commons.Tests.Utilities
{
  public class ExpressionUtilityTests
  {
    [Test]
    public void CreateSetterActionFromGetterExpression()
    {
      var dummy = new Dummy();

      // ACT
      var result = ExpressionUtility.CreateSetterActionFromGetterExpression((Dummy d) => dummy.Property);
      result(dummy, "Value");

      // ASSERT
      dummy.Property.Should().Be("Value");
    }

    [Test]
    public void CreateSetterActionFromGetterExpression_RetrievesResultFromCacheOnSecondCall()
    {
      var dummy = new Dummy();

      // ACT
      var result1 = ExpressionUtility.CreateSetterActionFromGetterExpression((Dummy d) => dummy.Property);
      var result2 = ExpressionUtility.CreateSetterActionFromGetterExpression((Dummy d) => dummy.Property);

      // ASSERT
      result1.Should().BeSameAs(result2);
    }

    [Test]
    public void CreateSetterActionFromGetterExpression_WithNonMemberExpression_Throws()
    {
      var dummy = new Dummy();

      // ACT
      Action act = () => ExpressionUtility.CreateSetterActionFromGetterExpression((Dummy d) => 5);

      // ASSERT
      act.ShouldThrowArgumentException("The given expression is not a valid property get expression.", "propertyGet");
    }

    private class Dummy
    {
      // ReSharper disable once UnusedAutoPropertyAccessor.Local
      public string Property { get; set; }
    }
  }
}
