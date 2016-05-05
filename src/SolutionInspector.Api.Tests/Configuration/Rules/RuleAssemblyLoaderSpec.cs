using System;
using System.IO;
using SystemInterface.IO;
using SystemInterface.Reflection;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

#region R# preamble for Machine.Specifications files

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

#endregion

namespace SolutionInspector.Api.Tests.Configuration.Rules
{
  [Subject (typeof (RuleAssemblyLoader))]
  class RuleAssemblyLoaderSpec
  {
    static IFile File;
    static IDirectory Directory;
    static IAssembly Assembly;

    static IAssembly LoadedAssembly;

    static RuleAssemblyLoader SUT;

    Establish ctx = () =>
    {
      File = A.Fake<IFile>();
      Directory = A.Fake<IDirectory>();
      Assembly = A.Fake<IAssembly>();

      LoadedAssembly = A.Fake<IAssembly>();
      A.CallTo(() => LoadedAssembly.GetExportedTypes()).Returns(new[] { typeof (DummyRule) });

      A.CallTo(() => Assembly.LoadFrom(A<string>._)).Returns(LoadedAssembly);

      SUT = new RuleAssemblyLoader(File, Directory, Assembly);
    };

    class when_loading_a_file
    {
      Because of = () => SUT.LoadRuleAssemblies(new[] { "File.dll" });

      It gets_file_attributes = () =>
          A.CallTo(() => File.GetAttributes("File.dll")).MustHaveHappened();

      It loads_assembly_from_file = () =>
          A.CallTo(() => Assembly.LoadFrom("File.dll")).MustHaveHappened();
    }

    class when_loading_a_directory
    {
      Establish ctx = () =>
      {
        A.CallTo(() => File.GetAttributes("Directory")).Returns(FileAttributes.Directory);
        A.CallTo(() => Directory.GetFiles(A<string>._, A<string>._)).Returns(new[] { @"Directory\File.dll" });
      };

      Because of = () => SUT.LoadRuleAssemblies(new[] { "Directory" });

      It gets_file_attributes = () =>
          A.CallTo(() => File.GetAttributes("Directory")).MustHaveHappened();

      It gets_assembly_files_from_directory = () =>
          A.CallTo(() => Directory.GetFiles("Directory", "*.dll")).MustHaveHappened();

      It loads_assembly_from_file = () =>
          A.CallTo(() => Assembly.LoadFrom(@"Directory\File.dll")).MustHaveHappened();
    }

    class when_loading_a_path_that_does_not_exist
    {
      Establish ctx = () =>
      {
        InnerException = new FileNotFoundException();
        A.CallTo(() => File.GetAttributes("DoesNotExist")).Throws(InnerException);
      };

      Because of = () => Exception = Catch.Exception(() => SUT.LoadRuleAssemblies(new[] { "DoesNotExist" }));

      It throws = () =>
          Exception.Should()
              .Be<RuleAssemblyNotFoundException>()
              .WithInnerException<FileNotFoundException>()
              .WithMessage("Could not find rule assembly 'DoesNotExist'.");

      static FileNotFoundException InnerException;
      static Exception Exception;
    }

    class when_loading_an_assembly_without_rules
    {
      Establish ctx = () =>
      {
        A.CallTo(() => LoadedAssembly.GetExportedTypes()).Returns(new[] { typeof (string) });
      };

      Because of = () => Exception = Catch.Exception(() => SUT.LoadRuleAssemblies(new[] { "File.dll" }));

      It throws = () =>
          Exception.Should()
              .Be<InvalidRuleAssemblyException>()
              .WithMessage("The assembly loaded from 'File.dll' is not a valid rule assembly.");

      static Exception Exception;
    }

    class when_assembly_loading_fails
    {
      Establish ctx = () =>
      {
        ThrownException = Some.Exception;
        A.CallTo(() => Assembly.LoadFrom(A<string>._)).Throws(ThrownException);
      };

      Because of = () => Exception = Catch.Exception(() => SUT.LoadRuleAssemblies(new[] { "File.dll" }));

      It throws = () =>
          Exception.Should()
              .Be<InvalidRuleAssemblyException>()
              .WithMessage("The assembly loaded from 'File.dll' is not a valid rule assembly.")
              .WithInnerException(ThrownException);

      static Exception ThrownException;
      static Exception Exception;
    }

    class DummyRule : IRule
    {

    }
  }
}