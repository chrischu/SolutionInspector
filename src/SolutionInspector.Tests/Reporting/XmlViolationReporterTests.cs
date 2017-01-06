using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Machine.Specifications;
using NUnit.Framework;
using SolutionInspector.Api.Rules;
using SolutionInspector.Reporting;
using SolutionInspector.TestInfrastructure.AssertionExtensions;

namespace SolutionInspector.Tests.Reporting
{
  public class XmlViolationReporterTests
  {
    static TextWriter _textWriter;
    static IRuleViolationViewModelConverter _ruleViolationViewModelConverter;

    static XmlViolationReporter _sut;

    [SetUp]
    public void SetUp ()
    {
      _textWriter = new StringWriter();
      _ruleViolationViewModelConverter = A.Fake<IRuleViolationViewModelConverter>();

      _sut = new XmlViolationReporter(_textWriter, _ruleViolationViewModelConverter);
    }

    [TearDown]
    public void TearDown ()
    {
       _sut.Dispose(); 
    }

    [Test]
    public void Report ()
    {
      var ruleViolations = A.Dummy<IEnumerable<IRuleViolation>>();
      var ruleViolationViewModels = new[]
                                {
                                      new RuleViolationViewModel(1, "Rule1", "Target1", "Message1"),
                                      new RuleViolationViewModel(2, "Rule2", "Target2", "Message2")
                                  };

      A.CallTo(() => _ruleViolationViewModelConverter.Convert(ruleViolations)).Returns(ruleViolationViewModels);

      // ACT
      _sut.Report(ruleViolations);

      // ASSERT
      _textWriter.ToString().Should().BeWithDiff(@"
<violations>
  <violation index=""1"" rule=""Rule1"" target=""Target1"">
    <message>Message1</message>
  </violation>
  <violation index=""2"" rule=""Rule2"" target=""Target2"">
    <message>Message2</message>
  </violation>
</violations>");
    }
  }
}