using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Primitives;
using JetBrains.Annotations;
using Wrapperator.Wrappers.IO;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  /// <summary>
  ///   Extension for easier assertion of objects.
  /// </summary>
  [PublicAPI]
  public static class ObjectStructuralEqualityExtensions
  {
    public static void ShouldAllBeEquivalentTo<T> (this IEnumerable<T> subject, params object[] expectation)
    {
      subject.ShouldAllBeEquivalentTo(expectation, config => config.RespectingRuntimeTypes().IgnoringCyclicReferences());
    }

    public static void ShouldAllBeEquivalentTo<T> (
      this IEnumerable<T> subject,
      Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config,
      params object[] expectation)
    {
      subject.ShouldAllBeEquivalentTo(expectation, config);
    }

    public static void ShouldAllBeLike<T> (this IEnumerable<T> subject, params object[] expectation)
    {
      subject.ShouldAllBeEquivalentTo(expectation, options => options.ExcludingMissingMembers());
    }

    public static void BeLike (this ObjectAssertions objectAssertions, object expectation)
    {
      objectAssertions.Subject.ShouldBeEquivalentTo(
        expectation,
        options =>
          options.ExcludingMissingMembers()
              .IgnoringCyclicReferences()
              .RespectingRuntimeTypes()
              .Using<FileInfoWrapper>(ctx => ctx.Subject.FullName.Should().Be(ctx.Expectation.FullName)).WhenTypeIs<FileInfoWrapper>()
              .Using<DirectoryInfoWrapper>(ctx => ctx.Subject.FullName.Should().Be(ctx.Expectation.FullName)).WhenTypeIs<DirectoryInfoWrapper>()
      );
    }
  }
}