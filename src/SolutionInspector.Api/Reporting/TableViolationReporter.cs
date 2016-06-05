using System;
using System.Collections.Generic;
using System.IO;
using SolutionInspector.Api.Rules;
using SolutionInspector.Api.Utilities;

namespace SolutionInspector.Api.Reporting
{
  internal class TableViolationReporter : ViolationReporterBase
  {
    private readonly IRuleViolationViewModelConverter _ruleViolationViewModelConverter;
    private readonly ITableWriter _tableWriter;

    public TableViolationReporter (TextWriter writer, IRuleViolationViewModelConverter ruleViolationViewModelConverter, ITableWriter tableWriter) : base(writer)
    {
      _ruleViolationViewModelConverter = ruleViolationViewModelConverter;
      _tableWriter = tableWriter;
    }

    protected override void Report (TextWriter writer, IEnumerable<IRuleViolation> violations)
    {
      var violationModels = _ruleViolationViewModelConverter.Convert(violations);
      _tableWriter.Write(writer, violationModels, v => v.Index, v => v.Rule, v => v.Target, v => v.Message);
    }
  }
}