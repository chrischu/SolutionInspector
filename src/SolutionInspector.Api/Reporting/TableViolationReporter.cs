using System;
using System.Collections.Generic;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.Reporting
{
  internal class TableViolationReporter : IViolationReporter
  {
    private readonly IRuleViolationViewModelConverter _ruleViolationViewModelConverter;
    private readonly IConsoleTableWriter _tableWriter;

    public TableViolationReporter(IRuleViolationViewModelConverter ruleViolationViewModelConverter, IConsoleTableWriter tableWriter)
    {
      _ruleViolationViewModelConverter = ruleViolationViewModelConverter;
      _tableWriter = tableWriter;
    }

    public void Report(IEnumerable<IRuleViolation> violations)
    {
      var violationModels = _ruleViolationViewModelConverter.Convert(violations);
      _tableWriter.Write(violationModels, v => v.Index, v => v.Rule, v => v.Target, v => v.Message);
    }
  }
}