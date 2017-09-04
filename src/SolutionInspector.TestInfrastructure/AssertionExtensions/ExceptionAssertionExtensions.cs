using System;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  /// <summary>
  ///   Extensions to make exception assertions easier.
  /// </summary>
  public static class ExceptionAssertionExtensions
  {
    /// <summary>
    ///   Checks that the given <paramref name="action" /> throws an <see cref="ArgumentException" /> with the given <paramref name="message" /> and
    ///   <paramref name="parameterName" />.
    /// </summary>
    public static void ShouldThrowArgumentException (this Action action, string message, string parameterName)
    {
      var expectedMessage = new ArgumentException(message, parameterName).Message;

      action.ShouldThrow<ArgumentException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///   Checks that the asserted <see cref="Exception" /> has an inner exception that is the same as <paramref name="innerException" />.
    /// </summary>
    // ReSharper disable once UnusedMethodReturnValue.Global
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