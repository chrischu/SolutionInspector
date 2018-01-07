using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using SolutionInspector.TestInfrastructure;
using Wrapperator.Interfaces;

namespace SolutionInspector.Commons.Console.Tests
{
  [TestFixture]
  public class ConsoleHelperTests
  {
    private IConsoleStatic _console;

    private IConsoleHelper _sut;

    [SetUp]
    public void SetUp ()
    {
      _console = A.Fake<IConsoleStatic>();

      _sut = new ConsoleHelper(_console);
    }

    [Test]
    [TestCase("y", true)]
    [TestCase("Y", true)]
    [TestCase("n", false)]
    [TestCase("N", false)]
    [TestCase("x", false)]
    [TestCase("yx", false)]
    [TestCase(null, false)]
    public void Confirm ([CanBeNull] string input, bool expectedResult)
    {
      var question = Some.String;

      A.CallTo(() => _console.ReadLine()).Returns(input);

      // ACT
      var result = _sut.Confirm(question);

      // ASSERT
      result.Should().Be(expectedResult);

      A.CallTo(() => _console.Write($"{question} [y/N] ")).MustHaveHappened();
    }
  }
}