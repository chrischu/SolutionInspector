using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commons.Utilities;

namespace SolutionInspector.Commons.Tests.Utilities
{
  public class CollectionDifferenceFinderTests
  {
    private CollectionDifferenceFinder _sut;

    [SetUp]
    public void SetUp()
    {
      _sut = new CollectionDifferenceFinder();
    }

    [Test]
    public void FindDifferences_WhenCollectionsAreEquivalent_FindsNoDifferences()
    {
      var collection1 = new[] { 7, 9 };
      var collection2 = new[] { 9, 7 };

      // ACT
      var result = _sut.FindDifferences(collection1, collection2);

      // ASSERT
      result.DifferencesFound.Should().BeFalse();
    }

    [Test]
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

    [Test]
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

    [Test]
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