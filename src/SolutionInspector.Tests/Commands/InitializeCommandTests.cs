using System.IO;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Commands;
using SolutionInspector.Commons.Console;
using SolutionInspector.TestInfrastructure.Console.Commands;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Tests.Commands
{
  public class InitializeCommandTests : CommandTestsBase
  {
    private IConsoleHelper _consoleHelper;
    private IFileStream _destinationStream;
    private IFileStatic _file;
    private IAssembly _resourceAssembly;

    private IStream _sourceStream;

    private InitializeCommand _sut;

    [SetUp]
    public new void SetUp ()
    {
      _resourceAssembly = A.Fake<IAssembly>();
      _file = A.Fake<IFileStatic>();
      _consoleHelper = A.Fake<IConsoleHelper>();

      _sourceStream = A.Fake<IStream>();
      A.CallTo(() => _resourceAssembly.GetManifestResourceStream(A<string>._)).Returns(_sourceStream);

      _destinationStream = A.Fake<IFileStream>();
      A.CallTo(() => _file.Open(A<string>._, A<FileMode>._)).Returns(_destinationStream);

      _sut = new InitializeCommand(_resourceAssembly, _file, _consoleHelper);
    }

    [Test]
    public void Run_Successfully_LoadsResourceAndCopiesItToFile ()
    {
      // ACT
      var result = RunCommand(_sut, "configFilePath");

      // ASSERT
      result.Should().Be(ConsoleConstants.SuccessExitCode);

      AssertConfigFileWrite();
    }

    [Test]
    public void Run_FileAlreadyExistsAndUserConfirmsOverwrite_OverwritesFile ()
    {
      A.CallTo(() => _file.Exists("configFilePath")).Returns(true);
      A.CallTo(() => _consoleHelper.Confirm(A<string>._)).Returns(true);

      // ACT
      var result = RunCommand(_sut, "configFilePath");

      // ASSERT
      A.CallTo(() => _consoleHelper.Confirm("File 'configFilePath' already exists. Do you want to overwrite it?")).MustHaveHappened();

      result.Should().Be(ConsoleConstants.SuccessExitCode);

      AssertConfigFileWrite();
    }

    [Test]
    public void Run_FileAlreadyExistsAndUserDoesNotConfirmOverwrite_AbortsCommand ()
    {
      A.CallTo(() => _file.Exists("configFilePath")).Returns(true);
      A.CallTo(() => _consoleHelper.Confirm(A<string>._)).Returns(false);

      // ACT
      var result = RunCommand(_sut, "configFilePath");

      // ASSERT
      A.CallTo(() => _consoleHelper.Confirm("File 'configFilePath' already exists. Do you want to overwrite it?")).MustHaveHappened();
      AssertCommandAbortion(result);
    }

    [Test]
    public void Run_FileAlreadyExistsButUserHasUsedForceFlags_OverwritesFile ()
    {
      A.CallTo(() => _file.Exists("configFilePath")).Returns(true);

      // ACT
      var result = RunCommand(_sut, "-f", "configFilePath");

      // ASSERT
      result.Should().Be(ConsoleConstants.SuccessExitCode);

      AssertConfigFileWrite();
    }

    private void AssertConfigFileWrite ()
    {
      A.CallTo(() => _resourceAssembly.GetManifestResourceStream("SolutionInspector.Template.SolutionInspectorConfig")).MustHaveHappened()
          .Then(A.CallTo(() => _file.Open("configFilePath", FileMode.Create)).MustHaveHappened())
          .Then(A.CallTo(() => _sourceStream.CopyTo(_destinationStream)).MustHaveHappened())
          .Then(A.CallTo(() => _destinationStream.Dispose()).MustHaveHappened())
          .Then(A.CallTo(() => _sourceStream.Dispose()).MustHaveHappened());
    }
  }
}