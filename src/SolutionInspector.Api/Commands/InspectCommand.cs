using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly IViolationReporterProxy _violationReporterProxy;

    public InspectCommand (
        ISolutionInspectorConfiguration configuration,
        ISolutionLoader solutionLoader,
        IRuleCollectionBuilder ruleCollectionBuilder,
        IViolationReporterProxy violationReporterProxy)
        : base("inspect", "Inspects a given solution for rule violations.")
    {
      _configuration = configuration;
      _solutionLoader = solutionLoader;
      _ruleCollectionBuilder = ruleCollectionBuilder;
      _violationReporterProxy = violationReporterProxy;
    }

    protected override void SetupArguments (IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Option<ViolationReportFormat>(
              "report",
              "r",
              "Writes a report of all violations to the console in the given format (Xml|Table|VisualStudio)",
              (a, v) => a.ReportFormat = v)
          .Values(c => c.Value("solutionFilePath", (a, v) => a.SolutionFilePath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments (RawArguments arguments, Func<string, Exception> reportError)
    {
      var solution = ValidateAndParseSolution(arguments, reportError);
      var rules = _ruleCollectionBuilder.Build(_configuration.Rules);

      return new ParsedArguments(solution, rules, arguments.ReportFormat);
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
            $"Given file '{arguments.SolutionFilePath}' is not a valid solution file " +
            "(for detailed error information see the log file 'SolutionInspector.log').");
      }
      catch (Exception ex)
      {
        s_logger.Error(ex, "Error while loading solution.");
        throw reportError($"Unexpected error when loading solution file '{arguments.SolutionFilePath}: {ex}");
      }
    }

    protected override int Run (ParsedArguments arguments)
    {
      s_logger.Info($"Inspecting solution '{arguments.Solution.FullPath}'...");

      var violations = GetRuleViolations(arguments.Solution, arguments.Rules).ToArray();

      if (violations.Any())
      {
        _violationReporterProxy.Report(arguments.ReportFormat, violations);
        return 1;
      }

      return 0;
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
    }

    public class ParsedArguments
    {
      public ISolution Solution { get; }
      public IRuleCollection Rules { get; }
      public ViolationReportFormat ReportFormat { get; }

      public ParsedArguments (ISolution solution, IRuleCollection rules, ViolationReportFormat reportFormat)
      {
        Solution = solution;
        Rules = rules;
        ReportFormat = reportFormat;
      }
    }
  }
}