using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Commons.Console;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Configuration.Validation;
using Wrapperator.Interfaces.Reflection;

namespace SolutionInspector.BuildTool.Commands
{
  internal class ValidateRuleAssemblyCommand : ConsoleCommandBase<ValidateRuleAssemblyCommand.RawArguments, ValidateRuleAssemblyCommand.ParsedArguments>
  {
    private readonly IAssemblyStatic _assembly;

    public ValidateRuleAssemblyCommand (IAssemblyStatic assembly)
        : base("validateRuleAssembly", "Validates a rule assembly.")
    {
      _assembly = assembly;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder.Values(c => c.Value("ruleAssemblyFilePath", (a, v) => a.RuleAssemblyPath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments)
    {
      var assembly = LoadAssembly(arguments.RuleAssemblyPath);
      var ruleTypes = assembly.GetTypes().Where(t => typeof(RuleConfigurationElement).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
          .ToList();

      if (ruleTypes.Count == 0)
        throw ReportArgumentValidationError($"Rule assembly '{arguments.RuleAssemblyPath}' does not contain a single rule");

      return new ParsedArguments(arguments.RuleAssemblyPath, ruleTypes);
    }

    private IAssembly LoadAssembly (string assemblyPath)
    {
      try
      {
        return _assembly.LoadFrom(assemblyPath);
      }
      catch (FileNotFoundException)
      {
        throw ReportArgumentValidationError($"Rule assembly '{assemblyPath}' could not be found");
      }
      catch (Exception ex)
      {
        throw ReportArgumentValidationError($"Unexpected error while loading rule assembly '{assemblyPath}'", ex);
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      var ruleTypesWithValidationErrors = new List<(Type ruleType, IReadOnlyDictionary<string, IReadOnlyCollection<string>> validationErrors)>();

      foreach (var ruleType in arguments.RuleTypes)
      {
        try
        {
          ConfigurationValidator.Validate(ruleType);
        }
        catch (ConfigurationValidationException ex)
        {
          Trace.Assert(ex.DocumentValidationErrors.Count == 0, "There should be no document validation errors here");
          ruleTypesWithValidationErrors.Add((ruleType, ex.PropertyValidationErrors));
        }
      }

      if (ruleTypesWithValidationErrors.Count == 0)
      {
        LogInfo($"Rule assembly '{arguments.RuleAssemblyPath}' is valid");
        return ConsoleConstants.SuccessExitCode;
      }

      var formattedValidationErrors = ruleTypesWithValidationErrors.FormatAsList(
          "Rule assembly is not valid. The following rule types had errors",
          ruleTypeWithValidationErrors => ruleTypeWithValidationErrors.validationErrors.FormatAsList(
              ruleTypeWithValidationErrors.ruleType.Name,
              propertiesWithValidationErrors => propertiesWithValidationErrors.Value.FormatAsList($"For property '{propertiesWithValidationErrors.Key}'")));

      LogError(formattedValidationErrors);
      return ConsoleConstants.ErrorExitCode;
    }

    public class RawArguments
    {
      [NotNull]
      // ReSharper disable once NotNullMemberIsNotInitialized (initialized by argument parsing)
      public string RuleAssemblyPath { get; set; }
    }

    public class ParsedArguments
    {
      public ParsedArguments (string ruleAssemblyPath, IReadOnlyList<Type> ruleTypes)
      {
        RuleAssemblyPath = ruleAssemblyPath;
        RuleTypes = ruleTypes;
      }

      [NotNull]
      public string RuleAssemblyPath { get; }

      [NotNull]
      public IReadOnlyList<Type> RuleTypes { get; }
    }
  }
}