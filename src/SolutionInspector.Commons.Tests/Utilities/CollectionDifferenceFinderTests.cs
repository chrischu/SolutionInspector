using FluentAssertions;
using SolutionInspector.Commons.Utilities;
using Xunit;

namespace SolutionInspector.Commons.Tests.Utilities
{
  public class CollectionDifferenceFinderTests
  {
    private readonly CollectionDifferenceFinder _sut;

    public CollectionDifferenceFinderTests ()
    {
      _sut = new CollectionDifferenceFinder();
    }

    [Fact]
    public void FindDifferences_WhenCollectionsAreEquivalent_FindsNoDifferences()
    {
      var collection1 = new[] { 7, 9 };
      var collection2 = new[] { 9, 7 };

      // ACT
      var result = _sut.FindDifferences(collection1, collection2);

      // ASSERT
      result.DifferencesFound.Should().BeFalse();
    }

    [Fact]
    public void FindDifferences_WhenSecondCollectionHasMoreElements_FindsAdds()
    {
      var collection1 = new[] { 7 };
      var collection2 = new[] { 9, 7 };

      // ACT
      var result = _sut.FindDifferences(collection1, collection2);

      // ASSERT
      result.DifferencesFound.Should().BeTrue();
      result.Adds.Should().Equal(9);
    }

    [Fact]
    public void FindDifferences_WhenSecondCollectionHasFewerElements_FindsRemoves()
    {
      var collection1 = new[] { 9, 7 };
      var collection2 = new[] { 7 };

      // ACT
      var result = _sut.FindDifferences(collection1, collection2);

      // ASSERT
      result.DifferencesFound.Should().BeTrue();
      result.Removes.Should().Equal(9);
    }

    [Fact]
    public void FindDifferences_WhenCollectionsDifferOnlyInAmountOfDuplicates_FindsNoDifferences()
    {
      var collection1 = new[] { 7 };
      var collection2 = new[] { 7, 7, 7 };

      // ACT
      var result = _sut.FindDifferences(collection1, collection2);

      // ASSERT
      result.DifferencesFound.Should().BeFalse();
    }
  }
}