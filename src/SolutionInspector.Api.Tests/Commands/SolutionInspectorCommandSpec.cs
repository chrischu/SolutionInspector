using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using ManyConsole;
using SolutionInspector.Api.Commands;

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
  [Subject (typeof(SolutionInspectorCommand<,>))]
  class SolutionInspectorCommandSpec
  {
    static IMsBuildInstallationChecker MsBuildInstallationChecker;
    static TextWriter TextWriter;

    static DummyCommand SUT;

    Establish ctx = () =>
    {
      MsBuildInstallationChecker = A.Fake<IMsBuildInstallationChecker>();

      TextWriter = A.Dummy<TextWriter>();

      SUT = new DummyCommand(MsBuildInstallationChecker);
    };

    class when_running_and_MsBuild_is_installed
    {
      Establish ctx = () => { A.CallTo(() => MsBuildInstallationChecker.IsMsBuildInstalled()).Returns(true); };

      Because of = () => Result = RunCommand(SUT);

      It runs_successfully = () =>
          Result.Should().Be(0);

      It calls_MsBuildInstallationChecker_IsMsBuildInstalled = () =>
          A.CallTo(() => MsBuildInstallationChecker.IsMsBuildInstalled()).MustHaveHappened();

      It does_not_call_MsBuildInstallationChecker_SuggestInstallation = () =>
          A.CallTo(() => MsBuildInstallationChecker.SuggestInstallation()).MustNotHaveHappened();

      static int Result;
    }

    class when_running_and_MsBuild_is_not_installed
    {
      Establish ctx = () => { A.CallTo(() => MsBuildInstallationChecker.IsMsBuildInstalled()).Returns(false); };

      Because of = () => Result = RunCommand(SUT);

      It runs_successfully = () =>
          Result.Should().Be(1);

      It calls_MsBuildInstallationChecker_IsMsBuildInstalled = () =>
          A.CallTo(() => MsBuildInstallationChecker.IsMsBuildInstalled()).MustHaveHappened();

      It does_not_call_MsBuildInstallationChecker_SuggestInstallation = () =>
          A.CallTo(() => MsBuildInstallationChecker.SuggestInstallation()).MustHaveHappened();

      static int Result;
    }

    static int RunCommand (ConsoleCommand command, params string[] arguments)
    {
      return ConsoleCommandDispatcher.DispatchCommand(command, arguments, TextWriter);
    }

    internal class DummyCommand : SolutionInspectorCommand<DummyCommand.RawArguments, DummyCommand.ParsedArguments>
    {
      public DummyCommand (IMsBuildInstallationChecker msBuildInstallationChecker)
          : base(msBuildInstallationChecker, "Dummy", "Dummy")
      {
      }

      protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
      {
      }

      protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
      {
        return new ParsedArguments();
      }

      protected override int Run (ParsedArguments arguments)
      {
        return 0;
      }

      public class RawArguments
      {
      }

      public class ParsedArguments
      {
      }
    }
  }
}