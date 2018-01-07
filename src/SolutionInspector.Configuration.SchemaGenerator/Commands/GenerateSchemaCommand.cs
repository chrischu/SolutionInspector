using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using JetBrains.Annotations;
using SolutionInspector.Commons.Console;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Interfaces.Xml;

namespace SolutionInspector.BuildTool.Commands
{
  internal class GenerateSchemaCommand : ConsoleCommandBase<GenerateSchemaCommand.RawArguments, GenerateSchemaCommand.ParsedArguments>
  {
    private readonly IFileStatic _file;
    private readonly IConsoleHelper _consoleHelper;
    private readonly IXmlWriterStatic _xmlWriter;
    private readonly IAssemblyStatic _assembly;
    private readonly IRuleAssemblySchemaCreator _ruleAssemblySchemaCreator;

    public GenerateSchemaCommand (
        IAssemblyStatic assembly,
        IFileStatic file,
        IConsoleHelper consoleHelper,
        IXmlWriterStatic xmlWriter,
        IRuleAssemblySchemaCreator ruleAssemblySchemaCreator)
        : base("generateSchema", "Generates a XSD schema for the rules contained in the specified assembly.")
    {
      _file = file;
      _consoleHelper = consoleHelper;
      _xmlWriter = xmlWriter;
      _assembly = assembly;
      _ruleAssemblySchemaCreator = ruleAssemblySchemaCreator;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Values(c => c.Value("assemblyFilePath", (a, v) => a.AssemblyFilePath = v))
          .Option<string>("baseSchemaVersion", "v", "The version of the base schema to reference in the created schema", (a, v) => a.BaseSchemaVersion = v)
          .Option<string>(
              "outputFilePath",
              "o",
              "A file path defining where the resulting XSD schema should be saved to. Defaults to '<assemblyName>.xsd'.",
              (a, v) => a.OutputFilePath = v)
          .Flag("force", "f", "Overwrite output file if it exists without asking for confirmation.", (a, v) => a.Force = v);
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments)
    {
      var assembly = LoadAssembly(arguments.AssemblyFilePath);

      var baseSchemaVersion = arguments.BaseSchemaVersion ?? BuildToolProgram.DefaultBaseSchemaVersion;
      var baseSchemaNamespace = string.Format(BuildToolProgram.BaseSchemaNamespaceTemplate, baseSchemaVersion);

      var outputFilePath = GetAndValidateOutputFilePath(arguments.OutputFilePath, arguments.AssemblyFilePath);

      return new ParsedArguments(assembly, baseSchemaNamespace, outputFilePath, arguments.Force);
    }

    private IAssembly LoadAssembly (string assemblyPath)
    {
      try
      {
        return _assembly.LoadFrom(assemblyPath);
      }
      catch (FileNotFoundException)
      {
        throw ReportArgumentValidationError($"Assembly '{assemblyPath}' could not be found");
      }
      catch (Exception ex)
      {
        throw ReportArgumentValidationError($"Unexpected error while loading assembly '{assemblyPath}'", ex);
      }
    }

    private string GetAndValidateOutputFilePath ([CanBeNull] string outputFilePath, string assemblyFilePath)
    {
      try
      {
        outputFilePath = outputFilePath ?? Path.ChangeExtension(assemblyFilePath, "xsd");

        if (!_file.Exists(outputFilePath))
        {
          Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputFilePath)).AssertNotNull());
          _file.WriteAllText(outputFilePath, "");
          _file.Delete(outputFilePath);
        }

        return outputFilePath;
      }
      catch (Exception ex)
      {
        throw ReportArgumentValidationError($"Invalid output path '{outputFilePath}'", ex);
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      if (_file.Exists(arguments.OutputFilePath) && !arguments.Force)
      {
        if(!_consoleHelper.Confirm($"File '{arguments.OutputFilePath}' already exists. Do you want to overwrite it?"))
          return ReportAbortion();
      }

      try
      {
        var schema = _ruleAssemblySchemaCreator.CreateSchema(arguments.RuleAssembly, arguments.BaseSchemaNamespace);

        using(var stream = _file.Create(arguments.OutputFilePath))
        using (var xmlWriter = _xmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
          schema.Write(xmlWriter._XmlWriter);

        return ConsoleConstants.SuccessExitCode;
      }
      catch (XmlSchemaException ex)
      {
        return ReportExecutionError("The generated schema is not valid", ex);
      }
      catch (Exception ex)
      {
        return ReportExecutionError("Unexpected error while generating schema", ex);
      }
    }

    public class RawArguments
    {
      [NotNull]
      // ReSharper disable once NotNullMemberIsNotInitialized (initialized by argument parsing)
      public string AssemblyFilePath { get; set; }

      [CanBeNull]
      public string BaseSchemaVersion { get; set; }

      [CanBeNull]
      public string OutputFilePath { get; set; }
      public bool Force { get; set; }
    }

    public class ParsedArguments
    {
      public ParsedArguments (IAssembly ruleAssembly, string baseSchemaNamespace, string outputFilePath, bool force)
      {
        RuleAssembly = ruleAssembly;
        BaseSchemaNamespace = baseSchemaNamespace;
        OutputFilePath = outputFilePath;
        Force = force;
      }

      [NotNull]
      public IAssembly RuleAssembly { get; }

      [NotNull]
      public string BaseSchemaNamespace { get; }

      [NotNull]
      public string OutputFilePath { get; }
      public bool Force { get; }
    }
  }
}