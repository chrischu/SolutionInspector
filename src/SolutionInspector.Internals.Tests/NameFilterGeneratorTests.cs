using System;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.Internals.Tests
{
  public class NameFilterGeneratorTests
  {
    [Test]
    public void Generate_AllSelected_GeneratesSingleAllInclude ()
    {
      // ACT
      var result = NameFilterGenerator.Generate(new[] { Tuple.Create(Some.String(), true) });

      // ASSERT
      result.Includes.Should().Equal("*");
      result.Excludes.Should().Equal();
    }

    [Test]
    public void Generate_NoneSelected_GeneratesSingleAllIncludeAndExclude ()
    {
      // ACT
      var result = NameFilterGenerator.Generate(new[] { Tuple.Create(Some.String(), false) });

      // ASSERT
      result.Includes.Should().Equal("*");
      result.Excludes.Should().Equal("*");
    }

    [Test]
    public void Generate_CommonPrefixInIncludes_GeneratesSingleIncludeWithPrefix ()
    {
      // ACT
      var result = NameFilterGenerator.Generate(
        new[]
        {
          Tuple.Create(Some.String(), false),
          Tuple.Create("Project.Web", true),
          Tuple.Create("Project.Console", true)
        });

      // ASSERT
      result.Includes.Should().Equal("Project.*");
      result.Excludes.Should().Equal();
    }

    [Test]
    public void Generate_CommonSuffixInIncludes_GeneratesSingleIncludeWithSuffix ()
    {
      // ACT
      var result = NameFilterGenerator.Generate(
        new[]
        {
          Tuple.Create(Some.String(), false),
          Tuple.Create("Some.Tests", true),
          Tuple.Create("Other.Tests", true)
        });

      // ASSERT
      result.Includes.Should().Equal("*.Tests");
      result.Excludes.Should().Equal();
    }

    [Test]
    public void Generate_MultiplePossibleAffixes_PicksBestOne ()                                                                                     
    {
      // ACT
      var result = NameFilterGenerator.Generate(
        new[]
        {
          Tuple.Create("B" + Some.String(), false),
          Tuple.Create("AAAX", true),
          Tuple.Create("AAY", true),
          Tuple.Create("AZ", true)
        });

      // ASSERT
      result.Includes.Should().Equal("A*");
      result.Excludes.Should().Equal();
    }

    [Test]
    public void Generate_GroupHasExactMatch_DoesNotIncludeAsterisk ()
    {
      // ACT
      var result = NameFilterGenerator.Generate(
        new[]
        {
          Tuple.Create("AB", true),
          Tuple.Create("X", false)
        });

      // ASSERT
      result.Includes.Should().Equal("AB");
      result.Excludes.Should().Equal();
    }

    [Test]
    public void Generate_Complex ()
    {
      // ACT
      var result = NameFilterGenerator.Generate(
        new[]
        {
          //Tuple.Create("SolutionInspector.Api", false),
          //Tuple.Create("SolutionInspector.Api.Tests", false),
          //Tuple.Create("SolutionInspector.TestInfrastructure", false),
          Tuple.Create("SolutionPackages", false),
          //Tuple.Create("SolutionInspector", false),
          Tuple.Create("SolutionInspector.DefaultRules", false),
          Tuple.Create("SolutionInspector.DefaultRules.Tests", false),
          //Tuple.Create("SolutionInspector.Tests", false),
          //Tuple.Create("SolutionInspector.Configuration", false),
          //Tuple.Create("SolutionInspector.Configuration.Tests", false),
          Tuple.Create("SolutionInspector.Internals", true),
          Tuple.Create("SolutionInspector.Internals.Tests", true)
          //Tuple.Create("SolutionInspector.Commons", false),
          //Tuple.Create("SolutionInspector.Commons.Tests", false),
          //Tuple.Create("SolutionInspector.Api.Configuration", false),
          //Tuple.Create("SolutionInspector.Api.Configuration.Tests", false),
          //Tuple.Create("SolutionInspector.ConfigurationUi", false),
          //Tuple.Create("SolutionInspector.ConfigurationUi.Tests", false)
        });

      // ASSERT
      result.Includes.Should().Equal("SolutionInspector.Internals*");
      result.Excludes.Should().Equal();
    }
  }
}