using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.AssertionExtensions;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.Internals.Tests
{
  public class RuleAssemblyLoaderTests
  {
    private IAssemblyStatic _assembly;
    private IDirectoryStatic _directory;
    private IFileStatic _file;
    private IAssembly _loadedAssembly;

    private RuleAssemblyLoader _sut;

    [SetUp]
    public void SetUp ()
    {
      _file = A.Fake<IFileStatic>();
      _directory = A.Fake<IDirectoryStatic>();
      _assembly = A.Fake<IAssemblyStatic>();

      _loadedAssembly = A.Fake<IAssembly>();
      A.CallTo(() => _loadedAssembly.GetExportedTypes()).Returns(new[] { typeof(DummyRule) });

      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Returns(_loadedAssembly);

      _sut = new RuleAssemblyLoader(_file, _directory, _assembly);
    }

    [Test]
    public void LoadRuleAssemblies_FromFile_LoadsAssemblyFromFile ()
    {
      // ACT
      _sut.LoadRuleAssemblies(new[] { "File.dll" });

      // ASSERT
      A.CallTo(() => _assembly.LoadFrom("File.dll")).MustHaveHappened();

      A.CallTo(() => _file.GetAttributes("File.dll")).MustHaveHappened();
    }

    [Test]
    public void LoadRuleAssemblies_FromDirectory_LoadsAllAssembliesFromDirectory ()
    {
      A.CallTo(() => _file.GetAttributes("Directory")).Returns(FileAttributes.Directory);
      A.CallTo(() => _directory.GetFiles(A<string>._, A<string>._)).Returns(new[] { @"Directory\File.dll", @"Directory\File2.dll" });

      // ACT
      _sut.LoadRuleAssemblies(new[] { "Directory" });

      // ASSERT
      A.CallTo(() => _assembly.LoadFrom(@"Directory\File.dll")).MustHaveHappened();
      A.CallTo(() => _assembly.LoadFrom(@"Directory\File2.dll")).MustHaveHappened();

      A.CallTo(() => _file.GetAttributes("Directory")).MustHaveHappened();
      A.CallTo(() => _directory.GetFiles("Directory", "*.dll")).MustHaveHappened();
    }

    [Test]
    public void LoadRuleAssembly_FromNonExistingFile_Throws ()
    {
      var fileNotFoundException = new FileNotFoundException();
      A.CallTo(() => _file.GetAttributes("DoesNotExist")).Throws(fileNotFoundException);

      // ACT
      Action act = () => _sut.LoadRuleAssemblies(new[] { "DoesNotExist" });

      // ASSERT
      act.ShouldThrow<RuleAssemblyNotFoundException>()
          .WithMessage("Could not find rule assembly 'DoesNotExist'.")
          .WithInnerException(fileNotFoundException);
    }

    [Test]
    public void LoadRuleAssembly_FromAssemblyWithoutRules_Throws ()
    {
      A.CallTo(() => _loadedAssembly.GetExportedTypes()).Returns(new[] { typeof(string) });

      // ACT
      Action act = () => _sut.LoadRuleAssemblies(new[] { "File.dll" });

      // ASSERT
      act.ShouldThrow<InvalidRuleAssemblyException>()
          .WithMessage("The assembly loaded from 'File.dll' is not a valid rule assembly.");
    }

    [Test]
    public void LoadRuleAssembly_WhenAssemblyLoadingFails_Throws ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Throws(thrownException);

      // ACT
      Action act = () => _sut.LoadRuleAssemblies(new[] { "File.dll" });

      // ASSERT
      act.ShouldThrow<InvalidRuleAssemblyException>()
          .WithMessage("The assembly loaded from 'File.dll' is not a valid rule assembly.")
          .WithInnerException(thrownException);
    }

    private class DummyRule : IRule
    {
    }
  }
}