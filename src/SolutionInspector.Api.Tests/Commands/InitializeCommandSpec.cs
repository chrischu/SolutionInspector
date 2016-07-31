using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using ManyConsole;
using SolutionInspector.Api.Commands;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

#region R# preamble for Machine.Specifications files

#pragma warning disable 414

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Commands
{
  [Subject (typeof(InitializeCommand))]
  class InitializeCommandSpec
  {
    static IAssembly ResourceAssembly;
    static IFileStatic File;
    static IConsoleStatic Console;

    static IStream SourceStream;
    static IFileStream DestinationStream;

    static TextWriter TextWriter;

    static InitializeCommand SUT;

    Establish ctx = () =>
    {
      ResourceAssembly = A.Fake<IAssembly>();
      File = A.Fake<IFileStatic>();
      Console = A.Fake<IConsoleStatic>();

      TextWriter = new StringWriter();

      SourceStream = A.Fake<IStream>();
      A.CallTo(() => ResourceAssembly.GetManifestResourceStream(A<string>._)).Returns(SourceStream);

      DestinationStream = A.Fake<IFileStream>();
      A.CallTo(() => File.Open(A<string>._, A<FileMode>._)).Returns(DestinationStream);

      SUT = new InitializeCommand(ResourceAssembly, File, Console);
    };

    class when_running_successfully
    {
      Because of = () => Result = RunCommand(SUT, "configFilePath");

      It returns_exit_code = () =>
          Result.Should().Be(0);

      It loads_resource_and_copies_it_to_file = () =>
          A.CallTo(() => ResourceAssembly.GetManifestResourceStream("Template.SolutionInspectorConfig")).MustHaveHappened()
              .Then(A.CallTo(() => File.Open("configFilePath", FileMode.Create)).MustHaveHappened())
              .Then(A.CallTo(() => SourceStream.CopyTo(DestinationStream)).MustHaveHappened())
              .Then(A.CallTo(() => DestinationStream.Dispose()).MustHaveHappened())
              .Then(A.CallTo(() => SourceStream.Dispose()).MustHaveHappened());

      static int Result;
    }

    class when_file_already_exists_and_user_confirms_overwrite
    {
      Establish ctx = () =>
      {
        A.CallTo(() => File.Exists("configFilePath")).Returns(true);
        A.CallTo(() => Console.ReadLine()).Returns("y");
      };

      Because of = () => Result = RunCommand(SUT, "configFilePath");

      It queries_for_confirmation = () =>
          A.CallTo(() => Console.WriteLine("File 'configFilePath' already exists. Do you want to overwrite it? [y/N]")).MustHaveHappened();

      It returns_exit_code = () =>
          Result.Should().Be(0);

      It loads_resource_and_copies_it_to_file = () =>
          A.CallTo(() => ResourceAssembly.GetManifestResourceStream("Template.SolutionInspectorConfig")).MustHaveHappened()
              .Then(A.CallTo(() => File.Open("configFilePath", FileMode.Create)).MustHaveHappened())
              .Then(A.CallTo(() => SourceStream.CopyTo(DestinationStream)).MustHaveHappened())
              .Then(A.CallTo(() => DestinationStream.Dispose()).MustHaveHappened())
              .Then(A.CallTo(() => SourceStream.Dispose()).MustHaveHappened());

      static int Result;
    }

    class when_file_already_exists_and_user_does_not_confirm_overwrite
    {
      Establish ctx = () =>
      {
        A.CallTo(() => File.Exists("configFilePath")).Returns(true);
        A.CallTo(() => Console.ReadLine()).Returns(string.Empty);
      };

      Because of = () => Result = RunCommand(SUT, "configFilePath");

      It queries_for_confirmation = () =>
          A.CallTo(() => Console.WriteLine("File 'configFilePath' already exists. Do you want to overwrite it? [y/N]")).MustHaveHappened();

      It confirms_command_abortion = () =>
          A.CallTo(() => Console.WriteLine("Command was aborted.")).MustHaveHappened();

      It returns_exit_code = () =>
          Result.Should().Be(1);

      static int Result;
    }

    class when_file_already_exists_but_user_has_used_force_flag
    {
      Establish ctx = () =>
      {
        A.CallTo(() => File.Exists("configFilePath")).Returns(true);
      };

      Because of = () => Result = RunCommand(SUT, "-f", "configFilePath");

      It returns_exit_code = () =>
          Result.Should().Be(0);

      It loads_resource_and_copies_it_to_file = () =>
          A.CallTo(() => ResourceAssembly.GetManifestResourceStream("Template.SolutionInspectorConfig")).MustHaveHappened()
              .Then(A.CallTo(() => File.Open("configFilePath", FileMode.Create)).MustHaveHappened())
              .Then(A.CallTo(() => SourceStream.CopyTo(DestinationStream)).MustHaveHappened())
              .Then(A.CallTo(() => DestinationStream.Dispose()).MustHaveHappened())
              .Then(A.CallTo(() => SourceStream.Dispose()).MustHaveHappened());
      static int Result;
    }

    static int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, TextWriter);
    }
  }
}