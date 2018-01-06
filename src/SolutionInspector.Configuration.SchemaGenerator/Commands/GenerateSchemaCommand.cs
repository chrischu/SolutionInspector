using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using JetBrains.Annotations;
using SolutionInspector.Commons.Console;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Reflection;
using Wrapperator.Interfaces.Xml;

namespace SolutionInspector.BuildTool.Commands
{
  internal class GenerateSchemaCommand : ConsoleCommandBase<GenerateSchemaCommand.RawArguments, GenerateSchemaCommand.ParsedArguments>
  {
    private readonly IFileStatic _file;
    private readonly IConsoleStatic _console;
    private readonly IXmlWriterStatic _xmlWriter;
    private readonly IAssemblyStatic _assembly;
    private readonly IRuleAssemblySchemaCreator _ruleAssemblySchemaCreator;

    public GenerateSchemaCommand (
        IAssemblyStatic assembly,
        IFileStatic file,
        IConsoleStatic console,
        IXmlWriterStatic xmlWriter,
        IRuleAssemblySchemaCreator ruleAssemblySchemaCreator)
        : base("generateSchema", "Generates a XSD schema for the rules contained in the specified assembly.")
    {
      _file = file;
      _console = console;
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

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
    {
      var assembly = LoadAssembly(arguments.AssemblyFilePath, reportError);

      var baseSchemaVersion = arguments.BaseSchemaVersion ?? BuildToolProgram.DefaultBaseSchemaVersion;
      var baseSchemaNamespace = string.Format(BuildToolProgram.BaseSchemaNamespaceTemplate, baseSchemaVersion);

      var outputFilePath = GetAndValidateOutputFilePath(arguments.OutputFilePath, arguments.AssemblyFilePath, reportError);

      return new ParsedArguments(assembly, baseSchemaNamespace, outputFilePath, arguments.Force);
    }

    private IAssembly LoadAssembly (string assemblyPath, Func<string, Exception> reportError)
    {
      try
      {
        return _assembly.LoadFrom(assemblyPath);
      }
      catch (FileNotFoundException)
      {
        throw reportError($"Assembly '{assemblyPath}' could not be found.");
      }
      catch (Exception ex)
      {
        throw reportError($"Unexpected error while loading assembly '{assemblyPath}': {ex.Message}.");
      }
    }

    private string GetAndValidateOutputFilePath ([CanBeNull] string outputFilePath, string assemblyFilePath, Func<string, Exception> reportError)
    {
      try
      {
        outputFilePath = outputFilePath ?? Path.ChangeExtension(assemblyFilePath, "xsd");

        if (!File.Exists(outputFilePath))
        {
          Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputFilePath)).AssertNotNull());
          File.WriteAllText(outputFilePath, "");
          File.Delete(outputFilePath);
        }

        return outputFilePath;
      }
      catch (Exception ex)
      {
        throw reportError($"Invalid output path '{outputFilePath}': {ex.Message}.");
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      if (_file.Exists(arguments.OutputFilePath) && !arguments.Force)
      {
        _console.Write($"File '{arguments.OutputFilePath}' already exists. Do you want to overwrite it? [y/N] ");
        var answer = _console.ReadLine();
        if (string.IsNullOrEmpty(answer) || !string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
        {
          _console.WriteLine("Command was aborted.");
          return 1;
        }
      }

      try
      {
        var schema = _ruleAssemblySchemaCreator.CreateSchema(arguments.RuleAssembly, arguments.BaseSchemaNamespace);

        using(var stream = _file.Create(arguments.OutputFilePath))
        using (var xmlWriter = _xmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
          schema.Write(xmlWriter._XmlWriter);

        return 0;
      }
      catch (XmlSchemaException ex)
      {
        _console.Error.WriteLine($"The generated schema is not valid: {ex.Message}.");
        return 1;
      }
      catch (Exception ex)
      {
        _console.Error.WriteLine($"Unexpected error while generating schema: {ex.Message}.");
        return 1;
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