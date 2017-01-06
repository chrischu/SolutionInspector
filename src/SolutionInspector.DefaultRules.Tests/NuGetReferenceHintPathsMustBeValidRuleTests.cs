using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Rules;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.DefaultRules.Tests
{
  public class NuGetReferenceHintPathsMustBeValidRuleTests
  {
    private IFileInfo _nuGetReferenceDllFile;
    private INuGetReference _nuGetReference;
    private IProject _project;

    private NuGetReferenceHintPathsMustBeValidRule _sut;

    [SetUp]
    public void SetUp ()
    {
      _project = A.Fake<IProject>();

      _nuGetReferenceDllFile = A.Fake<IFileInfo>();

      _nuGetReference = A.Fake<INuGetReference>();
      A.CallTo(() => _nuGetReference.DllFile).Returns(_nuGetReferenceDllFile);
      A.CallTo(() => _nuGetReference.HintPath).Returns("HintPath");
      A.CallTo(() => _nuGetReference.Package.Id).Returns("Id");

      A.CallTo(() => _project.NuGetReferences).Returns(new[] { _nuGetReference });

      _sut = new NuGetReferenceHintPathsMustBeValidRule();
    }

    [Test]
    public void Evaluate_AllNuGetReferenceHintPathsValid_ReturnsNoViolations ()
    {
      A.CallTo(() => _nuGetReferenceDllFile.Exists).Returns(true);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.Should().BeEmpty();
    }

    [Test]
    public void Evaluate_NuGetReferenceHintPathIsInvalid_ReturnsViolation ()
    {
      A.CallTo(() => _nuGetReferenceDllFile.Exists).Returns(false);

      // ACT
      var result = _sut.Evaluate(_project);

      // ASSERT
      result.ShouldBeEquivalentTo(
        new[]
        {
          new RuleViolation(_sut, _project, "The NuGet reference to package 'Id' has an invalid hint path ('HintPath').")
        });
    }
  }
}