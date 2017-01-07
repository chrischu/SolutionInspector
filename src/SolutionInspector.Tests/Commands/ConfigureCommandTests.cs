using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commands;
using SolutionInspector.TestInfrastructure;
using Wrapperator.Interfaces.Diagnostics;

namespace SolutionInspector.Tests.Commands
{
  public class ConfigureCommandTests : CommandTestsBase
  {
    private string _configurationUiPath;
    private IProcessStatic _processStatic;

    private ConfigureCommand _sut;

    [SetUp]
    public new void SetUp ()
    {
      _processStatic = A.Fake<IProcessStatic>();
      _configurationUiPath = Some.String();

      _sut = new ConfigureCommand(_configurationUiPath, _processStatic);
    }

    [Test]
    public void Run ()
    {
      var process = A.Fake<IProcess>();
      var exitCode = Some.PositiveInteger;
      A.CallTo(() => process.ExitCode).Returns(exitCode);

      A.CallTo(() => _processStatic.Start(A<string>._)).Returns(process);

      // ACT
      var result = RunCommand(_sut);

      // ASSERT
      result.Should().Be(exitCode);

      A.CallTo(() => _processStatic.Start(_configurationUiPath)).MustHaveHappened()
          .Then(A.CallTo(() => process.WaitForExit()).MustHaveHappened());
    }
  }
}