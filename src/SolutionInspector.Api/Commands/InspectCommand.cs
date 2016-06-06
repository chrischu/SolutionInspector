using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Exceptions;
using NLog;
using SolutionInspector.Api.Configuration;
using SolutionInspector.Api.ObjectModel;
using SolutionInspector.Api.Reporting;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.Commands
{
  internal class InspectCommand : SolutionInspectorCommand<InspectCommand.RawArguments, InspectCommand.ParsedArguments>
  {
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    private readonly ISolutionInspectorConfiguration _configuration;
    private readonly ISolutionLoader _solutionLoader;
    private readonly IRuleCollectionBuilder _ruleCollectionBuilder;
    private readonly IViolationReporterFactory _violationReporterFactory;

    public InspectCommand (
        IMsBuildInstallationChecker msBuildInstallationChecker,
        ISolutionInspectorConfiguration configuration,
        ISolutionLoader solutionLoader,
        IRuleCollectionBuilder ruleCollectionBuilder,
        IViolationReporterFactory violationReporterFactory)
        : base(msBuildInstallationChecker, "inspect", "Inspects a given solution for rule violations.")
    {
      _configuration = configuration;
      _solutionLoader = solutionLoader;
      _ruleCollectionBuilder = ruleCollectionBuilder;
      _violationReporterFactory = violationReporterFactory;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Option<ViolationReportFormat>(
              "reportFormat",
              "f",
              $"Controls the format of the violation report ({string.Join("|", Enum.GetNames(typeof(ViolationReportFormat)))}).",
              (a, v) => a.ReportFormat = v)
          .Option<string>(
              "reportOutputFile",
              "o",
              "Writes the violation report to the given file instead of to the console.",
              (a, v) => a.ReportOutputFile = v?.Trim())
          .Values(c => c.Value("solutionFilePath", (a, v) => a.SolutionFilePath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
    {
      var solution = ValidateAndParseSolution(arguments, reportError);
      var rules = _ruleCollectionBuilder.Build(_configuration.Rules);

      return new ParsedArguments(solution, rules, arguments.ReportFormat, arguments.ReportOutputFile);
    }

    private ISolution ValidateAndParseSolution (RawArguments arguments, Func<string, Exception> reportError)
    {
      try
      {
        return _solutionLoader.Load(arguments.SolutionFilePath, _configuration.MsBuildParsing);
      }
      catch (SolutionNotFoundException)
      {
        throw reportError($"Given solution file '{arguments.SolutionFilePath}' could not be found.");
      }
      catch (InvalidProjectFileException ex)
      {
        s_logger.Error(ex, "Error while loading solution.");
        throw reportError(
            $"Given solution file '{arguments.SolutionFilePath}' contains an invalid project file '{ex.ProjectFile}'." +
            "(for detailed error information see the log file 'SolutionInspector.log').");
      }
      catch (Exception ex)
      {
        s_logger.Error(ex, "Error while loading solution.");
        throw reportError($"Unexpected error when loading solution file '{arguments.SolutionFilePath}': {ex.Message}.");
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      using (var solution = arguments.Solution)
      {
        s_logger.Info($"Inspecting solution '{solution.FullPath}'...");

        var violations = GetRuleViolations(solution, arguments.Rules).ToArray();

        if (violations.Any())
        {
          using (var reporter = CreateViolationReporter(arguments))
            reporter.Report(violations);
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
      int previousViolationCount = 0;

      s_logger.Info($"Checking for solution rule violations in solution '{solution.FullPath}'...");
      foreach (var solutionRule in rules.SolutionRules)
        ruleViolations.AddRange(solutionRule.Evaluate(solution));
      s_logger.Info(
          $"Finished checking for solution rule violations in solution '{solution.FullPath}': " +
          $"Found {ruleViolations.Count - previousViolationCount} violations.");

      foreach (var project in solution.Projects)
      {
        s_logger.Info($"Checking for project rule violations in project '{project.FullPath}'...");
        previousViolationCount = ruleViolations.Count;
        foreach (var projectRule in rules.ProjectRules)
          ruleViolations.AddRange(projectRule.Evaluate(project));
        s_logger.Info(
            $"Finished checking for project rule violations in project '{project.FullPath}': " +
            $"Found {ruleViolations.Count - previousViolationCount} violations.");
      }

      foreach (var project in solution.Projects)
      {
        s_logger.Info($"Checking for project item rule violations in project '{project.FullPath}'...");
        previousViolationCount = ruleViolations.Count;

        foreach (var projectItem in project.ProjectItems)
        {
          s_logger.Debug($"Checking for project item rule violations in project item '{projectItem.FullPath}'...");
          foreach (var projectItemRule in rules.ProjectItemRules)
            ruleViolations.AddRange(projectItemRule.Evaluate(projectItem));
        }

        s_logger.Info(
            $"Finished checking for project item rule violations in project '{project.FullPath}': " +
            $"Found {ruleViolations.Count - previousViolationCount} violations.");
      }

      s_logger.Info(
          $"Finished checking for violations in solution '{solution.FullPath}': " +
          $"Found {ruleViolations.Count} total violations.");
      return ruleViolations;
    }

    public class RawArguments
    {
      public string SolutionFilePath { get; set; }
      public ViolationReportFormat ReportFormat { get; set; }
      public string ReportOutputFile { get; set; }
    }

    public class ParsedArguments
    {
      public ISolution Solution { get; }
      public IRuleCollection Rules { get; }
      public ViolationReportFormat ReportFormat { get; }
      public string ReportOutputFile { get; }

      public ParsedArguments (
          ISolution solution,
          IRuleCollection rules,
          ViolationReportFormat reportFormat,
          [CanBeNull] string reportOutputFile)
      {
        Solution = solution;
        Rules = rules;
        ReportFormat = reportFormat;
        ReportOutputFile = reportOutputFile;
      }
    }
  }
}