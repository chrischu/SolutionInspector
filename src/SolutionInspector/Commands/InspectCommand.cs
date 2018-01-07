using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Exceptions;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.Configuration.Ruleset;
using SolutionInspector.Api.Exceptions;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;
using SolutionInspector.Commons.Console;
using SolutionInspector.Commons.Extensions;
using SolutionInspector.Internals;
using SolutionInspector.Reporting;
using SolutionInspector.Rules;
using SolutionInspector.Utilities;

namespace SolutionInspector.Commands
{
  internal class InspectCommand : ConsoleCommandBase<InspectCommand.RawArguments, InspectCommand.ParsedArguments>
  {
    private readonly ISolutionInspectorConfiguration _configuration;

    private readonly IConfigurationLoader _configurationLoader;
    private readonly IRuleAssemblyLoader _ruleAssemblyLoader;
    private readonly IRuleCollectionBuilder _ruleCollectionBuilder;
    private readonly ISolutionLoader _solutionLoader;
    private readonly IViolationReporterFactory _violationReporterFactory;

    public InspectCommand (
        IConfigurationLoader configurationLoader,
        IRuleAssemblyLoader ruleAssemblyLoader,
        ISolutionLoader solutionLoader,
        IRuleCollectionBuilder ruleCollectionBuilder,
        IViolationReporterFactory violationReporterFactory,
        ISolutionInspectorConfiguration configuration)
        : base("inspect", "Inspects a given solution for rule violations.")
    {
      _configurationLoader = configurationLoader;
      _ruleAssemblyLoader = ruleAssemblyLoader;
      _solutionLoader = solutionLoader;
      _ruleCollectionBuilder = ruleCollectionBuilder;
      _violationReporterFactory = violationReporterFactory;
      _configuration = configuration;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Option<ViolationReportFormat>(
              "reportFormat",
              "f",
              $"Controls the format of the violation report ({Enum.GetNames(typeof(ViolationReportFormat)).Join("|")}).",
              (a, v) => a.ReportFormat = v)
          .Option<string>(
              "reportOutputFile",
              "o",
              "Writes the violation report to the given file instead of to the console.",
              (a, v) => a.ReportOutputFile = v?.Trim())
          .Option<string>(
              "configurationFile",
              "c",
              $"Changes the rule configuration file that is used to the given file path instead of <solutionFilePath>.{SolutionInspectorProgram.RulesetFileExtension}.",
              (a, v) => a.ConfigurationFile = v?.Trim()
          )
          .Values(c => c.Value("solutionFilePath", (a, v) => a.SolutionFilePath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments)
    {
      var configuration = ValidateAndLoadRulesConfiguration(arguments);
      var solution = ValidateAndParseSolution(arguments);
      var rules = _ruleCollectionBuilder.Build(configuration.Rules);

      return new ParsedArguments(solution, rules, arguments.ReportFormat, arguments.ReportOutputFile, configuration);
    }

    private IRulesetConfiguration ValidateAndLoadRulesConfiguration (RawArguments arguments)
    {
      var configurationFilePath = arguments.ConfigurationFile ?? $"{arguments.SolutionFilePath}.{SolutionInspectorProgram.RulesetFileExtension}";

      try
      {
        return _configurationLoader.LoadRulesConfig(configurationFilePath);
      }
      catch (FileNotFoundException)
      {
        throw ReportArgumentValidationError($"Could not find configuration file '{configurationFilePath}'");
      }
      catch (Exception ex)
      {
        throw ReportArgumentValidationError($"Unexpected error when loading configuration file '{configurationFilePath}'", ex);
      }
    }

    private ISolution ValidateAndParseSolution (RawArguments arguments)
    {
      try
      {
        return _solutionLoader.Load(arguments.SolutionFilePath, _configuration.MsBuildParsing);
      }
      catch (SolutionNotFoundException)
      {
        throw ReportArgumentValidationError($"Given solution file '{arguments.SolutionFilePath}' could not be found");
      }
      catch (InvalidProjectFileException ex)
      {
        throw ReportArgumentValidationError($"Given solution file '{arguments.SolutionFilePath}' contains an invalid project file '{ex.ProjectFile}'");
      }
      catch (Exception ex)
      {
        throw ReportArgumentValidationError($"Unexpected error when loading solution file '{arguments.SolutionFilePath}'", ex);
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      using (var solution = arguments.Solution)
      {
        LogInfo("Loading rule assemblies...");

        _ruleAssemblyLoader.LoadRuleAssemblies(arguments.Configuration.RuleAssemblyImports);

        LogInfo($"Inspecting solution '{solution.FullPath}'...");

        var violations = GetRuleViolations(solution, arguments.Rules).ToArray();

        if (violations.Any())
        {
          using (var reporter = CreateViolationReporter(arguments))
          {
            reporter.Report(violations);
          }

          return 1;
        }

        return 0;
      }
    }

    private IViolationReporter CreateViolationReporter (ParsedArguments arguments)
    {
      if (arguments.ReportOutputFile != null)
        return _violationReporterFactory.CreateFileReporter(arguments.ReportFormat, arguments.ReportOutputFile);

      return _violationReporterFactory.CreateConsoleReporter(arguments.ReportFormat);
    }

    private IEnumerable<IRuleViolation> GetRuleViolations (ISolution solution, IRuleCollection rules)
    {
      var ruleViolations = new List<IRuleViolation>();
      var previousViolationCount = 0;

      LogInfo($"Checking for solution rule violations in solution '{solution.FullPath}'...");
      foreach (var solutionRule in rules.SolutionRules)
        ruleViolations.AddRange(solutionRule.Evaluate(solution));
      LogInfo(
          $"Finished checking for solution rule violations in solution '{solution.FullPath}': " +
          $"Found {ruleViolations.Count - previousViolationCount} violations.");

      foreach (var project in solution.Projects)
      {
        LogInfo($"Checking for project rule violations in project '{project.FullPath}'...");
        previousViolationCount = ruleViolations.Count;
        foreach (var projectRule in rules.ProjectRules)
          ruleViolations.AddRange(projectRule.Evaluate(project));
        LogInfo(
            $"Finished checking for project rule violations in project '{project.FullPath}': " +
            $"Found {ruleViolations.Count - previousViolationCount} violations.");
      }

      foreach (var project in solution.Projects)
      {
        LogInfo($"Checking for project item rule violations in project '{project.FullPath}'...");
        previousViolationCount = ruleViolations.Count;

        foreach (var projectItem in project.ProjectItems)
        {
          LogDebug($"Checking for project item rule violations in project item '{projectItem.FullPath}'...");
          foreach (var projectItemRule in rules.ProjectItemRules)
            ruleViolations.AddRange(projectItemRule.Evaluate(projectItem));
        }

        LogInfo(
            $"Finished checking for project item rule violations in project '{project.FullPath}': " +
            $"Found {ruleViolations.Count - previousViolationCount} violations.");
      }

      LogInfo(
          $"Finished checking for violations in solution '{solution.FullPath}': " +
          $"Found {ruleViolations.Count} total violations.");
      return ruleViolations;
    }

    public class RawArguments
    {
      public string SolutionFilePath { get; set; }
      public ViolationReportFormat ReportFormat { get; set; }
      public string ReportOutputFile { get; set; }
      public string ConfigurationFile { get; set; }
    }

    public class ParsedArguments
    {
      public ParsedArguments (
          ISolution solution,
          IRuleCollection rules,
          ViolationReportFormat reportFormat,
          [CanBeNull] string reportOutputFile,
          IRulesetConfiguration configuration)
      {
        Solution = solution;
        Rules = rules;
        ReportFormat = reportFormat;
        ReportOutputFile = reportOutputFile;
        Configuration = configuration;
      }

      public ISolution Solution { get; }
      public IRuleCollection Rules { get; }
      public ViolationReportFormat ReportFormat { get; }

      [CanBeNull]
      public string ReportOutputFile { get; }

      public IRulesetConfiguration Configuration { get; }
    }
  }
}