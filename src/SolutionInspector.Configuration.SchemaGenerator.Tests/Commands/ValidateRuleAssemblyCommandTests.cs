using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;
using FakeItEasy;
using FluentAssertions;
using NLog;
using NUnit.Framework;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.BuildTool.Commands;
using SolutionInspector.Commons.Console;
using SolutionInspector.Configuration.Attributes;
using SolutionInspector.TestInfrastructure;
using SolutionInspector.TestInfrastructure.Console.Commands;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.BuildTool.Tests.Commands
{
  public class ValidateRuleAssemblyCommandTests : CommandTestsBase
  {
    private IAssemblyStatic _assembly;

    private ValidateRuleAssemblyCommand _sut;

    [SetUp]
    public new void SetUp ()
    {
      _assembly = A.Fake<IAssemblyStatic>();

      _sut = new ValidateRuleAssemblyCommand(_assembly);
    }

    [Test]
    public void Run_ValidatesRuleAssembly ()
    {
      var assembly = A.Fake<IAssembly>();
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Returns(assembly);
      A.CallTo(() => assembly.GetTypes()).Returns(new[] { typeof(ValidRule) });

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(ConsoleConstants.SuccessExitCode);

      A.CallTo(() => _assembly.LoadFrom("Assembly.dll")).MustHaveHappened();

      AssertLog(LogLevel.Info, "Rule assembly 'Assembly.dll' is valid");
    }

    [Test]
    public void Run_WithRuleAssemblyThatHasErrors_ReportsErrors()
    {
      var assembly = A.Fake<IAssembly>();
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Returns(assembly);
      A.CallTo(() => assembly.GetTypes()).Returns(new[] { typeof(InvalidRule) });

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);

      AssertLog(
          LogLevel.Error,
          @"Rule assembly is not valid. The following rule types had errors:
  - InvalidRule:
    - For property 'Value':
      - A required property must not have a default value.");
    }

    [Test]
    public void Run_WithRuleAssemblyWithoutRules_ReportsError ()
    {
      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);

      AssertLog(LogLevel.Error, "Rule assembly 'Assembly.dll' does not contain a single rule");
    }

    [Test]
    public void Run_WithRuleAssemblyThatDoesNotExist_ReportsError ()
    {
      var fileNotFoundException = new FileNotFoundException();
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Throws(fileNotFoundException);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);
      AssertErrorLog("Rule assembly 'Assembly.dll' could not be found");
    }

    [Test]
    public void Run_WhenSchemaOpeningFailsUnexpectedly_ReportsError ()
    {
      var thrownException = Some.Exception;
      A.CallTo(() => _assembly.LoadFrom(A<string>._)).Throws(thrownException);

      // ACT
      var result = RunCommand(_sut, "Assembly.dll");

      // ASSERT
      result.Should().Be(ConsoleConstants.ErrorExitCode);
      AssertErrorLog("Unexpected error while loading rule assembly 'Assembly.dll'", thrownException);
    }

    private class ValidRule : RuleConfigurationElement
    {
    }

    private class InvalidRule : RuleConfigurationElement
    {
      [ConfigurationValue(DefaultValue = "asdf")]
      public string Value { get; set; }
    }
  }
}