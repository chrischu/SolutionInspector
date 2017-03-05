using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class NameFilterTests
  {
    [Test]
    [TestCase (new[] { "Filter", "Filter" }, new[] { "Filter" }, TestName = "Remove duplicates")]
    [TestCase (new[] { "*", "Filter" }, new[] { "*" }, TestName = "Remove unnecessary filter")]
    [TestCase (new[] { "Fil***ter", "Fil**ter" }, new[] { "Fil*ter" }, TestName = "Collapse asterisks")]
    [TestCase (new[] { "A", "", "    " }, new[] { "A" }, TestName = "Removes empty or whitespace")]
    public void Ctor_SimplifiesIncludes (string[] filters, string[] expectedFilters)
    {
      // ACT
      var filter = new NameFilter(filters);

      // ASSERT
      filter.Includes.ShouldBeEquivalentTo(expectedFilters);
    }

    [Test]
    [TestCase(new[] { "Filter", "Filter" }, new[] { "Filter" }, TestName = "Remove duplicates")]
    [TestCase(new[] { "*", "Filter" }, new[] { "*" }, TestName = "Remove unnecessary filter")]
    [TestCase(new[] { "Fil***ter", "Fil**ter" }, new[] { "Fil*ter" }, TestName = "Collapse asterisks")]
    [TestCase(new[] { "A", "", "    " }, new[] { "A" }, TestName = "Removes empty or whitespace")]
    public void Ctor_SimplifiesExcludes(string[] filters, string[] expectedFilters)
    {
      // ACT
      var filter = new NameFilter(new[] { "X" }, filters);

      // ASSERT
      filter.Excludes.ShouldBeEquivalentTo(expectedFilters);
    }

    [Test]
    public void Ctor_WithoutIncludes_AddsDefaultAllInclude ()
    {
      // ACT
      var filter = new NameFilter(new string[0], new[] { "Filter" });

      // ASSERT
      filter.Includes.ShouldBeEquivalentTo(new[] { "*" });
      filter.Excludes.ShouldBeEquivalentTo(new[] { "Filter" });
    }

    [Test]
    public void Ctor_IfAllFiltersAreEmpty_AddsDefaultAllIncludeButNoExclude ()
    {
      // ACT
      var filter = new NameFilter(new[] { "   ", "" }, new[] { "   ", "" });

      // ASSERT
      filter.Includes.ShouldBeEquivalentTo(new[] { "*" });
      filter.Excludes.Should().BeEmpty();
    }

    [Test]
    public void Ctor_IfIncludeAndExcludeCancelEachOtherOut_RemovesCancellingIncludeAndExclude()
    {
      // ACT
      var filter = new NameFilter(new[] { "A", "B" }, new[] { "B" });

      // ASSERT
      filter.Includes.ShouldBeEquivalentTo(new[] { "A" });
      filter.Excludes.Should().BeEmpty();
    }

    [Test]
    [TestCase ("Include", "Include", true)]
    [TestCase ("Include", "NotIncluded", false)]
    [TestCase ("Inc*lude", "IncXYZlude", true)]
    [TestCase ("Inc*lude", "NotIncluded", false)]
    [TestCase ("+*Include;-ExcludedInclude", "XYZInclude", true)]
    [TestCase ("+*Include;-ExcludedInclude", "ExcludedInclude", false)]
    [TestCase ("+*Include;-Exc*ludedInclude", "XYZInclude", true)]
    [TestCase ("+*Include;-Exc*ludedInclude", "ExcXYZludedInclude", false)]
    public void IsMatch (string filterString, string testString, bool expectedResult)
    {
      var filter = NameFilter.Parse(filterString);

      // ACT
      var result = filter.IsMatch(testString);

      // ASSERT
      result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase("*", true)]
    [TestCase("A", false)]
    public void IncludesAll(string include, bool expectedResult)
    {
      var filter = new NameFilter(new[] { include }, new [] { "X" });

      // ACT
      var result = filter.IncludesAll;

      // ASSERT
      result.Should().Be(expectedResult);
    }

    [Test]
    public void SanitizesInputBeforeTurningIntoRegex ()
    {
      var filter = new NameFilter(new[] { "A.B" });

      // ACT
      var result = filter.IsMatch("AxB");

      // ASSERT
      result.Should().BeFalse();
    }
  }
}