using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Exceptions;
using SolutionInspector.Configuration;
using SolutionInspector.ObjectModel;
using SolutionInspector.Reporting;
using SolutionInspector.Rules;
using SolutionInspector.Utilities;

namespace SolutionInspector.Commands
{
  internal class InspectCommand : SolutionInspectorCommand<InspectCommand.RawArguments, InspectCommand.ParsedArguments>
  {
    private readonly ISolutionInspectorConfiguration _configuration;
    private readonly ISolutionLoader _solutionLoader;
    private readonly IRuleCollectionBuilder _ruleCollectionBuilder;
    private readonly IViolationReporterProxy _violationReporterProxy;

    public InspectCommand(
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

    protected override void SetupArguments(IArgumentsBuilder<RawArguments> argumentsBuilder)
    {
      argumentsBuilder
          .Option<ViolationReportFormat>(
              "report",
              "r",
              "Writes a report of all violations to the console in the given format (Xml|Table)",
              (a, v) => a.ReportFormat = v)
          .Values(c => c.Value("solutionFilePath", (a, v) => a.SolutionFilePath = v));
    }

    protected override ParsedArguments ValidateAndParseArguments(RawArguments arguments, Func<string, Exception> reportError)
    {
      var solution = ValidateAndParseSolution(arguments, reportError);
      var rules = _ruleCollectionBuilder.Build(_configuration.Rules);

      return new ParsedArguments(solution, rules, arguments.ReportFormat);
    }

    private ISolution ValidateAndParseSolution(RawArguments arguments, Func<string, Exception> reportError)
    {
      try
      {
        return _solutionLoader.Load(arguments.SolutionFilePath, _configuration.MsBuildParsing);
      }
      catch (FileNotFoundException)
      {
        throw reportError($"Given solution file '{arguments.SolutionFilePath}' could not be found.");
      }
      catch (InvalidProjectFileException)
      {
        throw reportError($"Given file '{arguments.SolutionFilePath}' is not a valid solution file.");
      }
    }

    protected override int Run(ParsedArguments arguments)
    {
      var violations = GetRuleViolations(arguments.Solution, arguments.Rules).ToArray();

      if (violations.Any())
      {
        if (arguments.ReportFormat != null)
          _violationReporterProxy.Report(arguments.ReportFormat.Value, violations);
        return 1;
      }

      return 0;
    }

    private IEnumerable<IRuleViolation> GetRuleViolations(ISolution solution, IRuleCollection rules)
    {
      var ruleViolations = new List<IRuleViolation>();

      foreach (var solutionRule in rules.SolutionRules)
        ruleViolations.AddRange(solutionRule.Evaluate(solution));

      foreach (var projectRule in rules.ProjectRules)
        foreach (var project in solution.Projects)
          ruleViolations.AddRange(projectRule.Evaluate(project));

      foreach(var projectItemRule in rules.ProjectItemRules)
        foreach(var project in solution.Projects)
          foreach (var projectItem in project.ProjectItems)
            ruleViolations.AddRange(projectItemRule.Evaluate(projectItem));

      return ruleViolations;
    }

    public class RawArguments
    {
      public string SolutionFilePath { get; set; }
      public ViolationReportFormat? ReportFormat { get; set; }
    }

    public class ParsedArguments
    {
      public ISolution Solution { get; }
      public IRuleCollection Rules { get; }
      public ViolationReportFormat? ReportFormat { get; }

      public ParsedArguments(ISolution solution, IRuleCollection rules, ViolationReportFormat? reportFormat)
      {
        Solution = solution;
        Rules = rules;
        ReportFormat = reportFormat;
      }
    }
  }
}