using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Infrastructure.Behaviors;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.ConfigurationUi.Tests.Infrastructure.Behaviors
{
  [Apartment (ApartmentState.STA)]
  public class PutCursorAtEndTextBoxBehaviorTests
  {
    private PutCursorAtEndTextBoxBehavior _sut;

    private TextBox _attachTarget;

    [SetUp]
    public void SetUp ()
    {
      _sut = new PutCursorAtEndTextBoxBehavior();

      _attachTarget = new TextBox();
      _sut.Attach(_attachTarget);

      _attachTarget.Text = Some.String();
      _attachTarget.CaretIndex = 0;

    }

    [Test]
    public void OnFocus_SetsCaretToEnd ()
    {
      // ACT
      _attachTarget.RaiseEvent(new RoutedEventArgs(UIElement.GotFocusEvent));

      // ASSERT
      _attachTarget.CaretIndex.Should().Be(_attachTarget.Text.Length);
    }

    [Test]
    public void Detaching_Works ()
    {
      // ACT
      _sut.Detach();
      _attachTarget.RaiseEvent(new RoutedEventArgs(UIElement.GotFocusEvent));

      // ASSERT
      _attachTarget.CaretIndex.Should().Be(0);
    }
  }
}