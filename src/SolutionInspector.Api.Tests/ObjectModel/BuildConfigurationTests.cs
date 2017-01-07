using System;
using System.Collections;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;

namespace SolutionInspector.Api.Tests.ObjectModel
{
  public class BuildConfigurationTests
  {
    [Test]
    public void Parsing ()
    {
      // ACT
      var result = BuildConfiguration.Parse("Debug|Any CPU");

      // ASSERT
      result.ConfigurationName.Should().Be("Debug");
      result.PlatformName.Should().Be("Any CPU");
    }

    [Test]
    public void Parsing_WithInvalidString_Throws ()
    {
      // ACT
      Action act = () => BuildConfiguration.Parse("NOT_VALID");

      // ASSERT
      act.ShouldThrow<FormatException>().WithMessage(@"The value 'NOT_VALID' is not a valid string representation of a BuildConfiguration.");
    }

    [Test]
    [TestCaseSource (nameof(EqualsTestData))]
    public bool Equals (BuildConfiguration a, [CanBeNull] BuildConfiguration b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsTestData ()
    {
      var buildConfiguration = new BuildConfiguration("A", "B");
      var buildConfigurationClone = new BuildConfiguration(buildConfiguration.ConfigurationName, buildConfiguration.PlatformName);

      yield return new TestCaseData(buildConfiguration, buildConfiguration) { ExpectedResult = true, TestName = "Equals_ReferenceEquality" };
      yield return new TestCaseData(buildConfiguration, buildConfigurationClone) { ExpectedResult = true };

      yield return new TestCaseData(buildConfiguration, new BuildConfiguration(buildConfiguration.ConfigurationName, "Y")) { ExpectedResult = false };
      yield return new TestCaseData(buildConfiguration, new BuildConfiguration("X", buildConfiguration.PlatformName)) { ExpectedResult = false };
      yield return new TestCaseData(buildConfiguration, null) { ExpectedResult = false };
    }

    [Test]
    [TestCaseSource (nameof(EqualsWithObjectsTestData))]
    public bool Equals_WithObjects (object a, [CanBeNull] object b)
    {
      // ACT & ASSERT
      return a.Equals(b);
    }

    private static IEnumerable EqualsWithObjectsTestData ()
    {
      var buildConfiguration = new BuildConfiguration("A", "B");

      yield return new TestCaseData(buildConfiguration, default(object)) { ExpectedResult = false };
      yield return new TestCaseData(buildConfiguration, buildConfiguration) { ExpectedResult = true };
    }
  }
}