using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.ConfigurationUi.Infrastructure.Behaviors;
using SolutionInspector.TestInfrastructure;

namespace SolutionInspector.ConfigurationUi.Tests.Infrastructure.Behaviors
{
  [Apartment (ApartmentState.STA)]
  public class IgnoreMousewheelBehaviorTests
  {
    private IgnoreMousewheelBehavior _sut;

    private UIElement _attachTarget;
    private MouseWheelEventHandler _mouseWheelEventHandler;

    [SetUp]
    public void SetUp ()
    {
      _sut = new IgnoreMousewheelBehavior();

      _attachTarget = new UIElement();
      _sut.Attach(_attachTarget);

      _mouseWheelEventHandler = A.Fake<MouseWheelEventHandler>();
      _attachTarget.MouseWheel += _mouseWheelEventHandler;
    }

    [Test]
    public void MouseWheelPreview_IsHandledAndForwarded ()
    {
      var eventArgs = new MouseWheelEventArgs(Mouse.PrimaryDevice, Some.PositiveInteger, Some.Integer) { RoutedEvent = Mouse.PreviewMouseWheelEvent };

      // ACT
      _attachTarget.RaiseEvent(eventArgs);

      // ASSERT
      eventArgs.Handled.Should().BeTrue();
      A.CallTo(
        () => _mouseWheelEventHandler(
          A<object>._,
          A<MouseWheelEventArgs>.That.Matches(m => m.MouseDevice == eventArgs.MouseDevice && m.Timestamp == eventArgs.Timestamp && m.Delta == eventArgs.Delta))
      ).MustHaveHappened();
    }

    [Test]
    public void Detaching_Works ()
    {
      _sut.Detach();

      var eventArgs = new MouseWheelEventArgs(Mouse.PrimaryDevice, Some.PositiveInteger, Some.Integer) { RoutedEvent = Mouse.PreviewMouseWheelEvent };

      // ACT
      _attachTarget.RaiseEvent(eventArgs);

      // ASSERT
      eventArgs.Handled.Should().BeFalse();
      A.CallTo(() => _mouseWheelEventHandler(A<object>._,A<MouseWheelEventArgs>._)).MustNotHaveHappened();
    }
  }
}