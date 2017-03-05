using System.Globalization;
using System.Windows;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Infrastructure.Converters;

namespace SolutionInspector.ConfigurationUi.Tests.Converters
{
  public class NotNullToVisibilityConverterTests
  {
    private NotNullToVisibilityConverter _sut;

    [SetUp]
    public void SetUp ()
    {
      _sut = new NotNullToVisibilityConverter();
    }

    [Test]
    [TestCase (null, Visibility.Collapsed)]
    [TestCase (1, Visibility.Visible)]
    public void Convert ([CanBeNull] object value, Visibility expectedResult)
    {
      // ACT
      var result = _sut.Convert(value, typeof(Visibility), null, CultureInfo.CurrentCulture);

      // ASSERT
      result.Should().Be(expectedResult);
    }

    [UsedImplicitly (ImplicitUseTargetFlags.WithMembers)]
    private enum DummyEnum
    {
      A,
      B
    }
  }
}