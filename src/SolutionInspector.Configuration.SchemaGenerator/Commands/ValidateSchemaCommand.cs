using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using JetBrains.Annotations;
using SolutionInspector.Commons.Console;
using SolutionInspector.Commons.Extensions;
using Wrapperator.Interfaces.IO;
using Wrapperator.Interfaces.Xml.Schema;

namespace SolutionInspector.BuildTool.Commands
{
  internal class ValidateSchemaCommand : ConsoleCommandBase<ValidateSchemaCommand.RawArguments, ValidateSchemaCommand.ParsedArguments>
  {
    private readonly IFileStatic _file;
    private readonly IXmlSchemaStatic _xmlSchema;

    public ValidateSchemaCommand (IFileStatic file, IXmlSchemaStatic xmlSchema)
        : base("validateSchema", "Validates a XSD file.")
    {
      _file = file;
      _xmlSchema = xmlSchema;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder.Values(c => c.Value("xsdFilePath", (a, v) => a.SchemaPath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments)
    {
      var fileStream = OpenFile(arguments.SchemaPath);
      return new ParsedArguments(arguments.SchemaPath, fileStream);
    }

    private IFileStream OpenFile (string schemaPath)
    {
      try
      {
        return _file.OpenRead(schemaPath);
      }
      catch (FileNotFoundException)
      {
        throw ReportArgumentValidationError($"Schema '{schemaPath}' could not be found");
      }
      catch (Exception ex)
      {
        throw ReportArgumentValidationError($"Unexpected error while opening schema from '{schemaPath}'", ex);
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      var validationEventArgs = new List<ValidationEventArgs>();
      _xmlSchema.Read(arguments.FileStream, (sender, args) => validationEventArgs.Add(args));

      if (validationEventArgs.Any())
      {
        var errorCount = validationEventArgs.Count(a => a.Severity == XmlSeverityType.Error);
        var warningCount = validationEventArgs.Count(a => a.Severity == XmlSeverityType.Warning);

        var formattedMessages = validationEventArgs.ConvertAndJoin(
            e => $"  - {e.Severity.ToString().ToUpper()} on ({e.Exception.LineNumber},{e.Exception.LinePosition}): {e.Message}",
            Environment.NewLine);

        if (errorCount > 0)
        {
          LogError($"Schema is not valid (found {errorCount} errors and {warningCount} warnings):{Environment.NewLine}{formattedMessages}");
          return ConsoleConstants.ErrorExitCode;
        }
        else
        {
          LogWarning($"Schema is valid but contains {warningCount} warnings:{Environment.NewLine}{formattedMessages}");
          return ConsoleConstants.SuccessExitCode;
        }
      }
      else
      {
        LogInfo($"Schema '{arguments.SchemaPath}' is valid");
        return ConsoleConstants.SuccessExitCode;
      }
    }

    public class RawArguments
    {
      [NotNull]
      // ReSharper disable once NotNullMemberIsNotInitialized (initialized by argument parsing)
      public string SchemaPath { get; set; }
    }

    public class ParsedArguments
    {
      public ParsedArguments (string schemaPath, IFileStream fileStream)
      {
        SchemaPath = schemaPath;
        FileStream = fileStream;
      }

      public string SchemaPath { get; }

      [NotNull]
      public IFileStream FileStream { get; }
    }
  }
}