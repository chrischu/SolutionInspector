using System.Collections.Generic;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Utilities;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Tests.Utilities
{
  [Subject(typeof (CollectionDifferenceFinder))]
  class CollectionDifferenceFinderSpec
  {
    static IEnumerable<int> Collection1;
    static IEnumerable<int> Collection2;

    static CollectionDifferenceFinder SUT;

    private Establish ctx = () => { SUT = new CollectionDifferenceFinder(); };

    class when_collections_are_equivalent
    {
      Establish ctx = () =>
      {
        Collection1 = new[] { 7 };
        Collection2 = new[] { 7 };
      };

      Because of = () => Result = SUT.FindDifferences(Collection1, Collection2);

      It finds_no_differences = () =>
          Result.DifferencesFound.Should().BeFalse();

      static CollectionDifferences<int> Result;
    }

    class when_one_collection_has_more_elements
    {
      Establish ctx = () =>
      {
        Collection1 = new int[0];
        Collection2 = new[] { 7 };
      };

      Because of = () => Result = SUT.FindDifferences(Collection1, Collection2);

      It finds_adds = () =>
          Result.Adds.Should().Equal(7);

      static CollectionDifferences<int> Result;
    }

    class when_one_collection_has_less_elements
    {
      Establish ctx = () =>
      {
        Collection1 = new[] { 7 };
        Collection2 = new int[0];
      };

      Because of = () => Result = SUT.FindDifferences(Collection1, Collection2);

      It finds_removes = () =>
          Result.Removes.Should().Equal(7);

      static CollectionDifferences<int> Result;
    }

    class when_collections_only_differ_in_duplicates
    {
      Establish ctx = () =>
      {
        Collection1 = new[] { 7 };
        Collection2 = new[] { 7, 7, 7 };
      };

      Because of = () => Result = SUT.FindDifferences(Collection1, Collection2);

      It finds_no_differences = () =>
          Result.DifferencesFound.Should().BeFalse();

      static CollectionDifferences<int> Result;
    }
  }
}