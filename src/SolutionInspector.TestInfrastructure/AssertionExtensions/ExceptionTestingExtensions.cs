using System;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;

namespace SolutionInspector.TestInfrastructure.AssertionExtensions
{
  public static class ExceptionTestingExtensions
  {
    public static ExceptionAssertions<TException> Be<TException>(this ObjectAssertions objectAssertions)
        where TException : Exception
    {
      objectAssertions.BeOfType<TException>();

      return new ExceptionAssertions<TException>(new[] { (TException) objectAssertions.Subject });
    }

    public static void BeArgumentException(this ObjectAssertions objectAssertions, string message, string parameterName)
    {
      var expectedMessage = new ArgumentException(message, parameterName).Message;

      objectAssertions.Be<ArgumentException>().WithMessage(expectedMessage);
    } 
  }
}