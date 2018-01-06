using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using SolutionInspector.BuildTool.Commands;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.Console;
using SolutionInspector.TestInfrastructure.Console.Commands;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Interfaces.Xml;
using Wrapperator.Wrappers;

namespace SolutionInspector.BuildTool.Tests.Commands
{
  public class GenerateSchemaCommandTests : CommandTestsBase
  {
    private IAssemblyStatic _assembly;
    private IFileStatic _file;
    private IConsoleStatic _console;
    private IXmlWriterStatic _xmlWriterStatic;
    private IRuleAssemblySchemaCreator _ruleAssemblySchemaCreator;

    private GenerateSchemaCommand _sut;
    private IAssembly _loadedAssembly;
    private XmlSchema _xmlSchema;
    private RecordingFileStream _outputStream;

    [SetUp]
    public new void SetUp ()
    {
      _assembly = A.Fake<IAssemblyStatic>();
      _file = A.Fake<IFileStatic>();
      _console = A.Fake<IConsoleStatic>();
      _xmlWriterStatic = A.Fake<IXmlWriterStatic>(o => o.Wrapping(Wrapper.XmlWriter));
      _ruleAssemblySchemaCreator = A.Fake<IRuleAssemblySchemaCreator>();

      _sut = new GenerateSchemaCommand(_assembly, _file, _console, _xmlWriterStatic, _ruleAssemblySchemaCreator);

      _loadedAssembly = A.Fake<IAssembly>();
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Returns(_loadedAssembly);

      _xmlSchema = new XmlSchema();
      A.CallTo(() => _ruleAssemblySchemaCreator.CreateSchema(A<IAssembly>._, A<string>._)).Returns(_xmlSchema);

      _outputStream = new RecordingFileStream();
      A.CallTo(() => _file.Create(A<string>._)).Returns(_outputStream);
    }

    [TearDown]
    public void TearDown ()
    {
      _outputStream?.Dispose();
    }

    [Test]
    public void Run_CreatesSchemaFile ()
    {
      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(0);
      _outputStream.ToString().Should().Be("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" />");

      A.CallTo(() => _assembly.LoadFrom("Assembly.dll")).MustHaveHappened();

      A.CallTo(
          () => _ruleAssemblySchemaCreator.CreateSchema(
              _loadedAssembly,
              string.Format(BuildToolProgram.BaseSchemaNamespaceTemplate, BuildToolProgram.DefaultBaseSchemaVersion))).MustHaveHappened();

      A.CallTo(() => _file.Create("Assembly.xsd")).MustHaveHappened();
      // ReSharper disable once AccessToDisposedClosure
      A.CallTo(() => _xmlWriterStatic.Create(_outputStream, A<XmlWriterSettings>.That.Matches(s => Equals(s.Encoding, Encoding.UTF8) && s.Indent)))
          .MustHaveHappened();
    }

    [Test]
    public void Run_WithArguments ()
    {
      // ACT
      var result = RunCommand(_sut, "--baseSchemaVersion=7", "--outputFilePath=Path\\out.xsd", "Assembly.dll");

      // ASSERT
      result.Should().Be(0);

      A.CallTo(() => _ruleAssemblySchemaCreator.CreateSchema(_loadedAssembly, string.Format(BuildToolProgram.BaseSchemaNamespaceTemplate, "7")))
          .MustHaveHappened();

      A.CallTo(() => _file.Create("Path\\out.xsd")).MustHaveHappened();
    }

    [Test]
    public void Run_WithAssemblyThatDoesNotExist_ReportsError ()
    {
      var fileNotFoundException = new FileNotFoundException();
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Throws(fileNotFoundException);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(-1);
      CapturedOutput.ToString().Should().Contain("Assembly 'Assembly.dll' could not be found.");
    }

    [Test]
    public void Run_WhenAssemblyLoadingFailsUnexpectedly_ReportsError ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Throws(thrownException);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(-1);
      CapturedOutput.ToString().Should().Contain($"Unexpected error while loading assembly 'Assembly.dll': {thrownException.Message}.");
    }

    [Test]
    public void Run_WithInvalidFilePath_ReportsError ()
    {
      // ACT
      var result = RunCommand(_sut, "<.dll");

      // ASSERT
      result.Should().Be(-1);
      CapturedOutput.ToString().Should().Contain("Invalid output path '': ");
    }

    [Test]
    public void Run_WhenCreatedSchemaIsInvalid_ReportsError ()
    {
      var schemaException = new XmlSchemaException(Some.String);
      A.CallTo(() => _ruleAssemblySchemaCreator.CreateSchema(A<IAssembly>._, A<string>._)).Throws(schemaException);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(1);
      A.CallTo(() => _console.Error.WriteLine($"The generated schema is not valid: {schemaException.Message}.")).MustHaveHappened();
    }

    [Test]
    public void Run_WhenSchemaCreationFailsUnexpectedly_ReportsError ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _ruleAssemblySchemaCreator.CreateSchema(A<IAssembly>._, A<string>._)).Throws(thrownException);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(1);
      A.CallTo(() => _console.Error.WriteLine($"Unexpected error while generating schema: {thrownException.Message}.")).MustHaveHappened();
    }

    [Test]
    public void Run_FileAlreadyExists_AndUserConfirmsOverwrite_OverwritesFile ()
    {
      A.CallTo(() => _file.Exists("Assembly.xsd")).Returns(true);
      A.CallTo(() => _console.ReadLine()).Returns("y");

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      A.CallTo(() => _console.Write("File 'Assembly.xsd' already exists. Do you want to overwrite it? [y/N] ")).MustHaveHappened();

      result.Should().Be(0);
      _outputStream.ToString().Should().Be("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" />");
    }

    [Test]
    public void Run_FileAlreadyExists_AndUserDoesNotConfirmOverwrite_AbortsCommand ()
    {
      A.CallTo(() => _file.Exists("Assembly.xsd")).Returns(true);
      A.CallTo(() => _console.ReadLine()).Returns(string.Empty);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      A.CallTo(() => _console.Write("File 'Assembly.xsd' already exists. Do you want to overwrite it? [y/N] ")).MustHaveHappened();
      A.CallTo(() => _console.WriteLine("Command was aborted.")).MustHaveHappened();

      result.Should().Be(1);
      _outputStream.ToString().Should().BeEmpty();
    }

    [Test]
    public void Run_FileAlreadyExistsButUserHasUsedForceFlags_OverwritesFile ()
    {
      A.CallTo(() => _file.Exists("Assembly.xsd")).Returns(true);

      // ACT
      var result = RunCommand(_sut, "-f", "Assembly.dll");

      // ASSERT
      result.Should().Be(0);
      _outputStream.ToString().Should().Be("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" />");
    }
  }
}