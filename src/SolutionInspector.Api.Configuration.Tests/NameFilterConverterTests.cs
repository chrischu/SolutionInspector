using System;
using FluentAssertions;
using NUnit.Framework;

namespace SolutionInspector.Api.Configuration.Tests
{
  public class NameFilterConverterTests
  {
    private readonly NameFilterConverter _sut;

    public NameFilterConverterTests ()
    {
      _sut = new NameFilterConverter();
    }

    [Test]
    public void ConvertTo ()
    {
      var filter = new NameFilter(new[] { "A", "*B", "C*", "*D*" }, new[] { "E", "*F", "G*", "*H*" });

      // ACT
      var result = _sut.ConvertTo(filter);

      // ASSERT
      result.Should().Be("+A;+*B;+C*;+*D*;-E;-*F;-G*;-*H*");
    }

    [Test]
    public void ConvertTo_WithNull_ReturnsNull ()
    {
      // ACT
      var result = _sut.ConvertTo(null);

      // ASSERT
      result.Should().BeNull();
    }

    [Test]
    public void ConvertFrom ()
    {
      var filter = new NameFilter(new[] { "A", "B" }, new[] { "B" });
      var filterString = _sut.ConvertTo(filter);

      // ACT
      var result = _sut.ConvertFrom(filterString);

      result.IsMatch("A").Should().BeTrue();
      result.IsMatch("B").Should().BeFalse();
      result.IsMatch("X").Should().BeFalse();
    }

    [Test]
    public void ConvertFrom_WithNull_ReturnsNull ()
    {
      // ACT
      var result = _sut.ConvertFrom(null);

      // ASSERT
      result.Should().BeNull();
    }

    [Test]
    public void ConvertFrom_WithInvalidFormat_Throws ()
    {
      var filterString = "THIS IS NOT A NAMEFILTER";

      // ACT
      Action act = () => _sut.ConvertFrom(filterString);

      // ASSERT
      act.ShouldThrow<FormatException>().WithMessage($"The filter string '{filterString}' is not in the correct format.");
    }
  }
}