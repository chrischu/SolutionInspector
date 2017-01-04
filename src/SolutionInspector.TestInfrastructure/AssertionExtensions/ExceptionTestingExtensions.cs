using System;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;
using JetBrains.Annotations;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  public static class ExceptionTestingExtensions
  {
    public static ExceptionAssertions<TException> Be<TException> (this ObjectAssertions objectAssertions)
        where TException : Exception
    {
      objectAssertions.BeOfType<TException>();

      return new ExceptionAssertions<TException>(new[] { (TException) objectAssertions.Subject });
    }

    public static void BeArgumentException (this ObjectAssertions objectAssertions, string message, string parameterName)
    {
      var expectedMessage = new ArgumentException(message, parameterName).Message;

      objectAssertions.Be<ArgumentException>().WithMessage(expectedMessage);
    }

    public static void ShouldThrowArgumentException(this Action action, string message, string parameterName)
    {
      var expectedMessage = new ArgumentException(message, parameterName).Message;

      action.ShouldThrow<ArgumentException>().WithMessage(expectedMessage);
    }

    [PublicAPI]
    public static ExceptionAssertions<TException> WithInnerException<TException> (
        this ExceptionAssertions<TException> exceptionAssertions,
        Exception innerException)
        where TException : Exception
    {
      exceptionAssertions.And.InnerException.Should().BeSameAs(innerException);
      return exceptionAssertions;
    }
  }
}