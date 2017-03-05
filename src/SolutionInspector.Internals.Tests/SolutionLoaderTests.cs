using System;
using System.IO;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.MsBuildParsing;
using SolutionInspector.Api.Exceptions;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.TestInfrastructure;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.Internals.Tests
{
  public class SolutionLoaderTests
  {
    private IFileStatic _file;

    private IMsBuildParsingConfiguration _msBuildParsingConfiguration;
    private SolutionLoader _sut;

    [SetUp]
    public void SetUp ()
    {
      _file = A.Fake<IFileStatic>();
      _sut = new SolutionLoader(_file);

      _msBuildParsingConfiguration = A.Dummy<IMsBuildParsingConfiguration>();
    }

    // a positive test is not really possible here since the private Solution.Load method is not fakeable, but the positive case is covered by the
    // SolutionTests (since the SolutionLoader does not have any special code for successful execution)

    [Test]
    public void Load_NonExistingSolution_Throws ()
    {
      // ACT
      Action act = () => Dev.Null = _sut.Load("DOESNOTEXIST", _msBuildParsingConfiguration);

      // ASSERT
      act.ShouldThrow<SolutionNotFoundException>().WithMessage("Could not find solution file at 'DOESNOTEXIST'.");
    }

    [Test]
    public void Load_ExistingSolution_LoadsSolution ()
    {
      var solutionPath = Path.Combine(
        Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath).AssertNotNull(),
        @"ObjectModel\TestData\Solution\TestSolution.sln");

      A.CallTo(() => _file.Exists(solutionPath)).Returns(true);

      // ACT
      var result = _sut.Load(solutionPath, _msBuildParsingConfiguration);

      // ASSERT
      result.Name.Should().Be("TestSolution");
    }
  }
}