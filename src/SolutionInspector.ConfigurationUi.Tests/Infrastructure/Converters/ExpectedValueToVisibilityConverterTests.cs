using System;
using System.Globalization;
using System.Windows;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Infrastructure.Converters;

namespace SolutionInspector.ConfigurationUi.Tests.Infrastructure.Converters
{
  public class ExpectedValueToVisibilityConverterTests
  {
    private ExpectedValueToVisibilityConverter _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new ExpectedValueToVisibilityConverter();
    }

    [Test]
    [TestCase (null, null, Visibility.Visible)]
    [TestCase (null, "", Visibility.Collapsed)]
    [TestCase ("", null, Visibility.Collapsed)]
    [TestCase ("A", "A", Visibility.Visible)]
    [TestCase ("A", "B", Visibility.Collapsed)]
    [TestCase (DummyEnum.A, "A", Visibility.Visible)]
    [TestCase (DummyEnum.A, "B", Visibility.Collapsed)]
    public void Convert ([CanBeNull] object value, [CanBeNull] object parameter, Visibility expectedResult)
    {
      // ACT
      var result = _sut.Convert(value, typeof(Visibility), parameter, CultureInfo.CurrentCulture);

      // ASSERT
      result.Should().Be(expectedResult);
    }

    private enum DummyEnum
    {
      A,

      [UsedImplicitly]
      B
    }
  }
}