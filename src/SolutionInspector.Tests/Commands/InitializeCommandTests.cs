using System.IO;
using FakeItEasy;
using FluentAssertions;
using ManyConsole;
using NUnit.Framework;
using SolutionInspector.Commands;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Tests.Commands
{
  public class InitializeCommandTests
  {
    private IAssembly _resourceAssembly;
    private IFileStatic _file;
    private IConsoleStatic _console;

    private IStream _sourceStream;
    private IFileStream _destinationStream;

    private TextWriter _textWriter;

    private InitializeCommand _sut;

    [SetUp]
    public void SetUp ()
    {
      _resourceAssembly = A.Fake<IAssembly>();
      _file = A.Fake<IFileStatic>();
      _console = A.Fake<IConsoleStatic>();

      _textWriter = new StringWriter();

      _sourceStream = A.Fake<IStream>();
      A.CallTo(() => _resourceAssembly.GetManifestResourceStream(A<string>._)).Returns(_sourceStream);

      _destinationStream = A.Fake<IFileStream>();
      A.CallTo(() => _file.Open(A<string>._, A<FileMode>._)).Returns(_destinationStream);

      _sut = new InitializeCommand(_resourceAssembly, _file, _console);
    }

    [Test]
    public void Run_Successfully_LoadsResourceAndCopiesItToFile ()
    {
      // ACT
      var result = RunCommand(_sut, "configFilePath");

      // ASSERT
      result.Should().Be(0);

      AssertConfigFileWrite();
    }

    [Test]
    public void Run_FileAlreadyExistsAndUserConfirmsOverwrite_OverwritesFile ()
    {
      A.CallTo(() => _file.Exists("configFilePath")).Returns(true);
      A.CallTo(() => _console.ReadLine()).Returns("y");

      // ACT
      var result = RunCommand(_sut, "configFilePath");

      // ASSERT
      A.CallTo(() => _console.Write("File 'configFilePath' already exists. Do you want to overwrite it? [y/N] ")).MustHaveHappened();

      result.Should().Be(0);

      AssertConfigFileWrite();
    }

    [Test]
    public void Run_FileAlreadyExistsAndUserDoesNotConfirmOverwrite_AbortsCommand ()
    {
      A.CallTo(() => _file.Exists("configFilePath")).Returns(true);
      A.CallTo(() => _console.ReadLine()).Returns(string.Empty);

      // ACT
      var result = RunCommand(_sut, "configFilePath");

      // ASSERT
      A.CallTo(() => _console.Write("File 'configFilePath' already exists. Do you want to overwrite it? [y/N] ")).MustHaveHappened();
      A.CallTo(() => _console.WriteLine("Command was aborted.")).MustHaveHappened();

      result.Should().Be(1);
    }

    [Test]
    public void Run_FileAlreadyExistsButUserHasUsedForceFlags_OverwritesFile ()
    {
      A.CallTo(() => _file.Exists("configFilePath")).Returns(true);

      // ACT
      var result = RunCommand(_sut, "-f", "configFilePath");

      // ASSERT
      result.Should().Be(0);

      AssertConfigFileWrite();
    }

    private int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, _textWriter);
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