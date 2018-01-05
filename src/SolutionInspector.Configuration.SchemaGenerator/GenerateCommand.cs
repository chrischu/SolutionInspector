using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Commons.Console;
using SolutionInspector.Configuration;
using SolutionInspector.Configuration.Validation;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.SchemaGenerator
{
  internal class GenerateCommand : ConsoleCommandBase<GenerateCommand.RawArguments, GenerateCommand.ParsedArguments>
  {
    private readonly IAssemblyStatic _assembly;

    public GenerateCommand (IAssemblyStatic assembly)
        : base("generate", "Generates a XSD schema for a configuration document specified by the assembly its contained in and optionally a partial type name.")
    {
      _assembly = assembly;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Values(c => c.Value("assemblyFilePath", (a, v) => a.AssemblyFilePath = v))
          .Option<string>(
              "configurationDocumentFilter",
              "f",
              "A partial type name of the type representing the configuration document.",
              (a, v) => a.PartialConfigurationDocumentTypeName = v?.Trim())
          .Option<string>(
              "outputFilePath",
              "o",
              "A file path defining where the resulting XSD schema should be saved to. Defaults to '<assemblyName>_<configurationDocumentTypeName>.xsd'.",
              (a, v) => a.OutputFilePath = v);
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
    {
      var assembly = LoadAssembly(arguments.AssemblyFilePath, reportError);
      var configurationDocumentType = LoadConfigurationDocumentType(assembly, arguments.PartialConfigurationDocumentTypeName, reportError);
      ValidateConfigurationDocumentType(configurationDocumentType, reportError);

      var outputFilePath = arguments.OutputFilePath ?? $"{assembly.GetName().Name}_{configurationDocumentType.Name}.xsd";
      ValidateOutputFilePath(outputFilePath, reportError);

      return new ParsedArguments(configurationDocumentType, outputFilePath);
    }

    private void ValidateOutputFilePath (string outputFilePath, Func<string, Exception> reportError)
    {
      try
      {
        File.WriteAllText(outputFilePath, "");
      }
      catch (Exception ex)
      {
        throw reportError($"Error while creating output at '{outputFilePath}': {ex.Message}.");
      }
    }

    private IAssembly LoadAssembly (string assemblyPath, Func<string, Exception> reportError)
    {
      try
      {
        return _assembly.LoadFrom(assemblyPath);
      }
      catch (FileNotFoundException)
      {
        throw reportError($"Given assembly '{assemblyPath}' could not be found.");
      }
      catch (Exception ex)
      {
        throw reportError($"Unexpected error while loading assembly: {ex.Message}.");
      }
    }

    private Type LoadConfigurationDocumentType (
        IAssembly assembly,
        [CanBeNull] string partialConfigurationDocumentTypeName,
        Func<string, Exception> reportError)
    {
      var availableConfigurationDocumentTypes = assembly.GetTypes().Where(t => typeof(ConfigurationDocument).IsAssignableFrom(t)).ToList();

      if (partialConfigurationDocumentTypeName == null)
      {
        if (availableConfigurationDocumentTypes.Count == 1)
          return availableConfigurationDocumentTypes[0];

        var availableTypeNames = string.Join(", ", availableConfigurationDocumentTypes.Select(t => $"'{t.Name}'"));
        throw reportError(
            $"There are multiple configuration document types in the assembly: {availableTypeNames}. " +
            "Pick one by specifying the 'configurationDocumentFilter' parameter when running the program.");
      }
      else
      {
        var matchingConfigurationDocumentTypes = availableConfigurationDocumentTypes.Where(t => t.Name.Contains(partialConfigurationDocumentTypeName)).ToList();

        if (matchingConfigurationDocumentTypes.Count == 1)
          return matchingConfigurationDocumentTypes[0];

        var matchingTypeNames = string.Join(", ", availableConfigurationDocumentTypes.Select(t => $"'{t.Name}'"));
        throw reportError(
            "There are multiple configuration document types in the assembly matching " +
            $"the specified filter '{partialConfigurationDocumentTypeName}: {matchingTypeNames}. " +
            "Please specify a 'configurationDocumentFilter' that only applies to exactly one configuration document type.");
      }
    }

    private void ValidateConfigurationDocumentType (Type configurationDocumentType, Func<string, Exception> reportError)
    {
      try
      {
        ConfigurationValidator.Validate(configurationDocumentType);
      }
      catch (ConfigurationValidationException ex)
      {
        throw reportError($"Configuration documentation type '{configurationDocumentType.Name} is not valid: {ex.Message}");
      }
      catch (Exception ex)
      {
        throw reportError($"Unexpected error while validating configuration document type: {ex.Message}.");
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      throw new NotImplementedException();
    }

    public class RawArguments
    {
      public string AssemblyFilePath { get; set; }

      [CanBeNull]
      public string PartialConfigurationDocumentTypeName { get; set; }

      [CanBeNull]
      public string OutputFilePath { get; set; }
    }

    public class ParsedArguments
    {
      public ParsedArguments (Type configurationDocumentType, string outputFilePath)
      {
        ConfigurationDocumentType = configurationDocumentType;
        OutputFilePath = outputFilePath;
      }

      public Type ConfigurationDocumentType { get; }
      public string OutputFilePath { get; }
    }
  }
}