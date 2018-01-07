using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.BuildTool.Commands;
using SolutionInspector.Commons.Console;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.Console.Commands;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Xml.Schema;

namespace SolutionInspector.BuildTool.Tests.Commands
{
  public class ValidateSchemaCommandTests : CommandTestsBase
  {
    private IFileStatic _file;
    private IXmlSchemaStatic _xmlSchema;

    private ValidateSchemaCommand _sut;

    [SetUp]
    public new void SetUp ()
    {
      _file = A.Fake<IFileStatic>();
      _xmlSchema = A.Fake<IXmlSchemaStatic>();

      _sut = new ValidateSchemaCommand(_file, _xmlSchema);
    }

    [Test]
    public void Run_ValidatesSchemaFile ()
    {
      // ACT
      var result = RunCommand(_sut, "Schema.xsd");

      // ASSERT
      result.Should().Be(ConsoleConstants.SuccessExitCode);

      A.CallTo(() => _file.OpenRead("Schema.xsd")).MustHaveHappened();

      AssertLog(NLog.LogLevel.Info, "Schema 'Schema.xsd' is valid");
    }

    [Test]
    public void Run_WithSchemaThatHasWarnings_ReportsWarnings ()
    {
      A.CallTo(() => _xmlSchema.Read(A<IStream>._, A<ValidationEventHandler>._)).Invokes(
          (IStream stream, ValidationEventHandler handler) =>
          {
            handler(null, CreateValidationEvent(XmlSeverityType.Warning, "Warning1", 13, 37));
            handler(null, CreateValidationEvent(XmlSeverityType.Warning, "Warning2", 47, 11));
          });

      // ACT
      var result = RunCommand(_sut, "Schema.xsd");

      // ASSERT
      result.Should().Be(ConsoleConstants.SuccessExitCode);

      AssertLog(
          NLog.LogLevel.Warn,
          @"Schema is valid but contains 2 warnings:
  - WARNING on (13,37): Warning1
  - WARNING on (47,11): Warning2");
    }

    [Test]
    public void Run_WithSchemaThatHasErrorsAndWarnings_ReportsBothInTheOrderTheyWereReported ()
    {
      A.CallTo(() => _xmlSchema.Read(A<IStream>._, A<ValidationEventHandler>._)).Invokes(
          (IStream stream, ValidationEventHandler handler) =>
          {
            handler(null, CreateValidationEvent(XmlSeverityType.Warning, "Warning1", 13, 37));
            handler(null, CreateValidationEvent(XmlSeverityType.Error, "Error", 42, 24));
            handler(null, CreateValidationEvent(XmlSeverityType.Warning, "Warning2", 47, 11));
          });

      // ACT
      var result = RunCommand(_sut, "Schema.xsd");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);

      AssertLog(
          NLog.LogLevel.Error,
          @"Schema is not valid (found 1 errors and 2 warnings):
  - WARNING on (13,37): Warning1
  - ERROR on (42,24): Error
  - WARNING on (47,11): Warning2");
    }

    [Test]
    public void Run_WithSchemaThatDoesNotExist_ReportsError ()
    {
      var fileNotFoundException = new FileNotFoundException();
      A.CallTo(() => _file.OpenRead(A<string>._)).Throws(fileNotFoundException);

      // ACT
      var result = RunCommand(_sut, "Schema.xsd");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);
      AssertErrorLog("Schema 'Schema.xsd' could not be found");
    }

    [Test]
    public void Run_WhenSchemaOpeningFailsUnexpectedly_ReportsError ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _file.OpenRead(A<string>._)).Throws(thrownException);

      // ACT
      var result = RunCommand(_sut, "Schema.xsd");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);
      AssertErrorLog("Unexpected error while opening schema from 'Schema.xsd'", thrownException);
    }

    private ValidationEventArgs CreateValidationEvent (XmlSeverityType severityType, string message, int lineNumber, int linePosition)
    {
      var xmlSchemaException = new XmlSchemaException(message, null, lineNumber, linePosition);
      var xmlSeverityType = severityType;

      var ctor = typeof(ValidationEventArgs).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single(c => c.GetParameters().Length == 2);
      return (ValidationEventArgs) ctor.Invoke(new object[] { xmlSchemaException, xmlSeverityType });
    }
  }
}